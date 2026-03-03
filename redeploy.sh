#!/bin/bash
set -euo pipefail

# =============================================================================
# Redeploy script for BankSystem API
#
# Rebuilds the Docker image, pushes it to ACR, and updates the Container App.
#
# Usage:
#   chmod +x redeploy.sh
#   ./redeploy.sh
#
# Prerequisites:
#   - Azure CLI installed and logged in (az login)
#   - Resources already provisioned via provision-azure.sh
# =============================================================================

# ── Configuration (must match provision-azure.sh) ─────────────────────────────
RESOURCE_GROUP="banksystem-rg"
ACR_NAME="banksystemacr"
CONTAINER_APP_NAME="banksystem-api"
IMAGE_NAME="banksystem-api"
IMAGE_TAG="latest"

# ── Verify Azure CLI login ─────────────────────────────────────────────────────
echo "▶ Checking Azure CLI login..."
az account show > /dev/null 2>&1 || { echo "Not logged in. Run 'az login' first."; exit 1; }

ACR_LOGIN_SERVER=$(az acr show --name "$ACR_NAME" --query loginServer -o tsv)
FULL_IMAGE="$ACR_LOGIN_SERVER/$IMAGE_NAME:$IMAGE_TAG"

# ── Build and push image ───────────────────────────────────────────────────────
echo ""
echo "▶ Building and pushing Docker image to ACR..."
az acr build \
  --registry "$ACR_NAME" \
  --image "$IMAGE_NAME:$IMAGE_TAG" \
  --file BankSystem.Api/Dockerfile \
  .

# ── Update Container App ───────────────────────────────────────────────────────
echo ""
echo "▶ Updating Container App..."
az containerapp update \
  --resource-group "$RESOURCE_GROUP" \
  --name "$CONTAINER_APP_NAME" \
  --image "$FULL_IMAGE" \
  --output none

APP_URL=$(az containerapp show \
  --resource-group "$RESOURCE_GROUP" \
  --name "$CONTAINER_APP_NAME" \
  --query properties.configuration.ingress.fqdn -o tsv)

# ── Done ───────────────────────────────────────────────────────────────────────
echo ""
echo "✔ Redeploy complete!"
echo ""
echo "  API URL: https://$APP_URL"
echo ""
