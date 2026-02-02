using Microsoft.AspNetCore.Mvc;

namespace BankRUs.Api.Endpoints;

public static class CustomersEndpoints
{
    public static void MapCustomersEndpoints(this WebApplication app)
    {
      

        // GET /api/customers
        app.MapGet("/api/customers", () =>
        {
            // Vi skickar in en referens till en List<Customer> i minnet, ramverk
            // (ASP.NET Core) skapar från denna en serialiserad representation 
            // av informationen i JSON-format.

            // 200 OK
            return Results.Ok(customers);
        });

        // GET /api/customers/1
        app.MapGet("/api/customers/{id}", (int id) =>
        {
            var customer = customers.Find(x => x.Id == id);

            if (customer is null)
            {
                return Results.NotFound();
            }

            // Vi skickar in en referens till en List<Customer> i minnet, ramverk
            // (ASP.NET Core) skapar från denna en serialiserad representation 
            // av informationen i JSON-format.

            // 200 OK
            return Results.Ok(customer);
        });


        // DELETE /api/customers/1
        app.MapDelete("/api/customers/{id}", (int id) =>
        {
            var customer = customers.Find(x => x.Id == id);

            if (customer is null)
            {
                // 404 Not Found
                return Results.NotFound();
            }

            customers.Remove(customer);

            // 204 No Content
            return Results.NoContent();
        });

        // POST /api/customers
        app.MapPost("/api/customers", async (
            HttpRequest req,

            [FromBody] Customer customer) =>
        {
            var newCustomer = new Customer
            {
                Id = customers.Count + 1,
                FirstName = customer.FirstName,
                LastName = customer.LastName
            };

            customers.Add(newCustomer);

            // SÄTT BREAKPOINT
            // Returnera 201 Created
            return Results.Created("", newCustomer);
        });

    }
}
