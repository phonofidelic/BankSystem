#!/bin/bash
set -euo pipefail

# =============================================================================
# Azure resource provisioning script for BankSystem API
#
# Resources created:
#   - Resource Group
#   - Azure Container Registry (ACR)
#   - Azure SQL Server + Database
#   - Azure Key Vault
#   - User-assigned Managed Identity
#   - Container Apps Environment
#   - Container App
#
# Usage:
#   chmod +x provision-azure.sh
#   ./provision-azure.sh
#
# Prerequisites:
#   - Azure CLI installed and logged in (az login)
#   - Docker installed and running
# =============================================================================

# ── Configuration ─────────────────────────────────────────────────────────────
LOCATION="swedencentral"
RESOURCE_GROUP="banksystem-rg"
ACR_NAME="banksystemacr"                      # Must be globally unique, lowercase, alphanumeric
SQL_SERVER_NAME="banksystem-sql"              # Must be globally unique
SQL_DB_NAME="BankSystem"
SQL_ADMIN_USER="banksystemadmin"
KEY_VAULT_NAME="banksystem-kv"               # Must be globally unique
MANAGED_IDENTITY_NAME="banksystem-identity"
CONTAINER_ENV_NAME="banksystem-env"
CONTAINER_APP_NAME="banksystem-api"
IMAGE_NAME="banksystem-api"
IMAGE_TAG="latest"

# ── Prompt for secrets (never hard-coded) ─────────────────────────────────────
echo ""
echo "Enter values for secrets (input is hidden where sensitive):"
echo ""
read -rsp "  SQL admin password:       " SQL_ADMIN_PASSWORD; echo
read -rsp "  JWT secret:               " JWT_SECRET; echo
read -rsp "  Default admin password:   " DEFAULT_ADMIN_PASSWORD; echo
read -rp  "  Default admin username:   " DEFAULT_ADMIN_USERNAME
read -rp  "  Default admin email:      " DEFAULT_ADMIN_EMAIL
echo ""

# ── Verify Azure CLI login ─────────────────────────────────────────────────────
echo "▶ Checking Azure CLI login..."
az account show > /dev/null 2>&1 || { echo "Not logged in. Run 'az login' first."; exit 1; }
SUBSCRIPTION_ID=$(az account show --query id -o tsv)
echo "  Using subscription: $SUBSCRIPTION_ID"

# ── Resource Group ─────────────────────────────────────────────────────────────
echo ""
echo "▶ Creating resource group..."
az group create \
  --name "$RESOURCE_GROUP" \
  --location "$LOCATION" \
  --output none

# ── Azure Container Registry ───────────────────────────────────────────────────
echo ""
echo "▶ Creating Azure Container Registry..."
az acr create \
  --resource-group "$RESOURCE_GROUP" \
  --name "$ACR_NAME" \
  --sku Basic \
  --admin-enabled false \
  --output none

ACR_LOGIN_SERVER=$(az acr show --name "$ACR_NAME" --query loginServer -o tsv)
echo "  ACR login server: $ACR_LOGIN_SERVER"

# ── Build and push Docker image ────────────────────────────────────────────────
echo ""
echo "▶ Building and pushing Docker image to ACR..."
az acr build \
  --registry "$ACR_NAME" \
  --image "$IMAGE_NAME:$IMAGE_TAG" \
  --file BankSystem.Api/Dockerfile \
  .

FULL_IMAGE="$ACR_LOGIN_SERVER/$IMAGE_NAME:$IMAGE_TAG"

# ── Azure SQL Server and Database ──────────────────────────────────────────────
echo ""
echo "▶ Creating Azure SQL Server..."
az sql server create \
  --resource-group "$RESOURCE_GROUP" \
  --name "$SQL_SERVER_NAME" \
  --location "$LOCATION" \
  --admin-user "$SQL_ADMIN_USER" \
  --admin-password "$SQL_ADMIN_PASSWORD" \
  --output none

echo "▶ Allowing Azure services to access SQL Server..."
az sql server firewall-rule create \
  --resource-group "$RESOURCE_GROUP" \
  --server "$SQL_SERVER_NAME" \
  --name "AllowAzureServices" \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0 \
  --output none

echo "▶ Creating SQL Database..."
az sql db create \
  --resource-group "$RESOURCE_GROUP" \
  --server "$SQL_SERVER_NAME" \
  --name "$SQL_DB_NAME" \
  --edition GeneralPurpose \
  --family Gen5 \
  --capacity 2 \
  --compute-model Serverless \
  --auto-pause-delay 60 \
  --output none

SQL_CONNECTION_STRING="Server=tcp:${SQL_SERVER_NAME}.database.windows.net,1433;Database=${SQL_DB_NAME};User ID=${SQL_ADMIN_USER};Password=${SQL_ADMIN_PASSWORD};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

# ── Key Vault ──────────────────────────────────────────────────────────────────
echo ""
if az keyvault show --name "$KEY_VAULT_NAME" --resource-group "$RESOURCE_GROUP" --output none 2>/dev/null; then
  echo "▶ Key Vault already exists, skipping creation."
else
  echo "▶ Creating Key Vault..."
  az keyvault create \
    --resource-group "$RESOURCE_GROUP" \
    --name "$KEY_VAULT_NAME" \
    --location "$LOCATION" \
    --enable-rbac-authorization true \
    --output none
fi

KEY_VAULT_URI=$(az keyvault show --name "$KEY_VAULT_NAME" --query properties.vaultUri -o tsv)

# Grant current user access to set secrets
CURRENT_USER_ID=$(az ad signed-in-user show --query id -o tsv)
KEY_VAULT_RESOURCE_ID=$(az keyvault show --name "$KEY_VAULT_NAME" --query id -o tsv)

az role assignment create \
  --assignee "$CURRENT_USER_ID" \
  --role "Key Vault Secrets Officer" \
  --scope "$KEY_VAULT_RESOURCE_ID" \
  --output none

echo "▶ Storing secrets in Key Vault..."
az keyvault secret set --vault-name "$KEY_VAULT_NAME" --name "ConnectionStrings--Default"    --value "$SQL_CONNECTION_STRING"  --output none
az keyvault secret set --vault-name "$KEY_VAULT_NAME" --name "JwtOptions--Secret"            --value "$JWT_SECRET"             --output none
az keyvault secret set --vault-name "$KEY_VAULT_NAME" --name "DefaultAdmin--Username"        --value "$DEFAULT_ADMIN_USERNAME" --output none
az keyvault secret set --vault-name "$KEY_VAULT_NAME" --name "DefaultAdmin--Password"        --value "$DEFAULT_ADMIN_PASSWORD" --output none
az keyvault secret set --vault-name "$KEY_VAULT_NAME" --name "DefaultAdmin--Email"           --value "$DEFAULT_ADMIN_EMAIL"    --output none

# ── User-assigned Managed Identity ────────────────────────────────────────────
echo ""
echo "▶ Creating managed identity..."
az identity create \
  --resource-group "$RESOURCE_GROUP" \
  --name "$MANAGED_IDENTITY_NAME" \
  --output none

IDENTITY_RESOURCE_ID=$(az identity show --resource-group "$RESOURCE_GROUP" --name "$MANAGED_IDENTITY_NAME" --query id -o tsv)
IDENTITY_PRINCIPAL_ID=$(az identity show --resource-group "$RESOURCE_GROUP" --name "$MANAGED_IDENTITY_NAME" --query principalId -o tsv)
IDENTITY_CLIENT_ID=$(az identity show --resource-group "$RESOURCE_GROUP" --name "$MANAGED_IDENTITY_NAME" --query clientId -o tsv)

echo "▶ Granting managed identity access to Key Vault secrets..."
az role assignment create \
  --assignee-object-id "$IDENTITY_PRINCIPAL_ID" \
  --assignee-principal-type ServicePrincipal \
  --role "Key Vault Secrets User" \
  --scope "$KEY_VAULT_RESOURCE_ID" \
  --output none

echo "▶ Granting managed identity access to pull from ACR..."
ACR_RESOURCE_ID=$(az acr show --name "$ACR_NAME" --query id -o tsv)
az role assignment create \
  --assignee-object-id "$IDENTITY_PRINCIPAL_ID" \
  --assignee-principal-type ServicePrincipal \
  --role "AcrPull" \
  --scope "$ACR_RESOURCE_ID" \
  --output none

# ── Container Apps Environment ─────────────────────────────────────────────────
echo ""
echo "▶ Creating Container Apps environment..."
az containerapp env create \
  --resource-group "$RESOURCE_GROUP" \
  --name "$CONTAINER_ENV_NAME" \
  --location "$LOCATION" \
  --output none

# ── Container App ──────────────────────────────────────────────────────────────
echo ""
echo "▶ Creating Container App..."

# Key Vault secret references — mapped to .NET env var names (: replaced with __)
az containerapp create \
  --resource-group "$RESOURCE_GROUP" \
  --name "$CONTAINER_APP_NAME" \
  --environment "$CONTAINER_ENV_NAME" \
  --image "$FULL_IMAGE" \
  --registry-server "$ACR_LOGIN_SERVER" \
  --registry-identity "$IDENTITY_RESOURCE_ID" \
  --user-assigned "$IDENTITY_RESOURCE_ID" \
  --target-port 8080 \
  --ingress external \
  --min-replicas 1 \
  --max-replicas 3 \
  --secrets \
    "connectionstring=keyvaultref:${KEY_VAULT_URI}secrets/ConnectionStrings--Default,identityref:${IDENTITY_RESOURCE_ID}" \
    "jwt-secret=keyvaultref:${KEY_VAULT_URI}secrets/JwtOptions--Secret,identityref:${IDENTITY_RESOURCE_ID}" \
    "admin-username=keyvaultref:${KEY_VAULT_URI}secrets/DefaultAdmin--Username,identityref:${IDENTITY_RESOURCE_ID}" \
    "admin-password=keyvaultref:${KEY_VAULT_URI}secrets/DefaultAdmin--Password,identityref:${IDENTITY_RESOURCE_ID}" \
    "admin-email=keyvaultref:${KEY_VAULT_URI}secrets/DefaultAdmin--Email,identityref:${IDENTITY_RESOURCE_ID}" \
  --env-vars \
    "ASPNETCORE_ENVIRONMENT=Production" \
    "ConnectionStrings__Default=secretref:connectionstring" \
    "JwtOptions__Secret=secretref:jwt-secret" \
    "DefaultAdmin__Username=secretref:admin-username" \
    "DefaultAdmin__Password=secretref:admin-password" \
    "DefaultAdmin__Email=secretref:admin-email" \
  --output none

APP_URL=$(az containerapp show --resource-group "$RESOURCE_GROUP" --name "$CONTAINER_APP_NAME" --query properties.configuration.ingress.fqdn -o tsv)

# ── Done ───────────────────────────────────────────────────────────────────────
echo ""
echo "✔ Provisioning complete!"
echo ""
echo "  API URL:      https://$APP_URL"
echo "  ACR:          $ACR_LOGIN_SERVER"
echo "  Key Vault:    $KEY_VAULT_URI"
echo "  SQL Server:   ${SQL_SERVER_NAME}.database.windows.net"
echo ""
echo "  To redeploy the image after a code change:"
echo "    az acr build --registry $ACR_NAME --image $IMAGE_NAME:$IMAGE_TAG --file BankSystem.Api/Dockerfile ."
echo "    az containerapp update --name $CONTAINER_APP_NAME --resource-group $RESOURCE_GROUP --image $FULL_IMAGE"
