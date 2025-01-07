using Microsoft.EntityFrameworkCore;
using Data_Access_Layer.ApplicationDbContext;
using Data_Access_Layer.Uow;
using Data_Access_Layer.Entities;
using Business_Logic_Layer.Services;
using Business_Logic_Layer.ServicesInterfaces;
using Task.Middlewares;
//using API.Middleware;
using Business_Logic_Layer.DTO;
using API.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using AspNetCoreRateLimit;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace Task
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region Add Logger

            #endregion

            #region Jason
            var builder = WebApplication.CreateBuilder(args);
            var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

            // Ensure the log directory exists
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Configure Serilog to log to both console and rolling log files
            Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.Console(outputTemplate: "{Message}{NewLine}")
        .WriteTo.File(Path.Combine(Directory.GetCurrentDirectory(), "logs", "log-.txt"),
                      rollingInterval: RollingInterval.Day,
                      outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
        .CreateLogger();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()   // Allow all origins
                          .AllowAnyMethod()   // Allow all HTTP methods (GET, POST, etc.)
                          .AllowAnyHeader();  // Allow all headers
                });
            });

            builder.Logging.ClearProviders(); // Clear default providers
            builder.Logging.AddSerilog(); // Add Serilog as the logger provider


            // Use Serilog for logging
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

            #endregion

            #region Logging

            builder.Services.AddHttpLogging(logging =>
            {
                logging.LoggingFields = HttpLoggingFields.ResponseBody; // You might want to reduce this to avoid logging large bodies
                logging.RequestBodyLogLimit = 4096;  // Limit the size of the request body logged
                logging.ResponseBodyLogLimit = 4096;  // Limit the size of the response body logged
            });
            builder.Services.AddLogging();  // Ensure logging services are added

            #endregion
            #region AddResponseCompression
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            builder.Services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });
            #endregion
            builder.Services.AddMemoryCache();
            builder.Services.AddHttpContextAccessor();

            #region Ratelimit And HealthCheck
            builder.Services.AddInMemoryRateLimiting();
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>(); 
            builder.Services.Configure<IpRateLimitOptions>(options =>
            {
                options.EnableEndpointRateLimiting = true;
                options.StackBlockedRequests = false;
                options.GeneralRules = new List<RateLimitRule>
                {
                    new RateLimitRule
                    {
                        Endpoint = "*",
                        Limit = 100, 
                        Period = "1m" 
                    }
                };
            });

            builder.Services.AddHealthChecks();
            #endregion

            #region Authentication And Authorization
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
{
    var secretKey = builder.Configuration["Jwt:Key"];
    var issuer = builder.Configuration["Jwt:Issuer"];
    var audience = builder.Configuration["Jwt:Audience"];

    if (string.IsNullOrEmpty(secretKey))
    {
        throw new InvalidOperationException("JWT SecretKey is not configured in appsettings.json.");
    }

    if (string.IsNullOrEmpty(issuer))
    {
        throw new InvalidOperationException("JWT Issuer is not configured in appsettings.json.");
    }

    if (string.IsNullOrEmpty(audience))
    {
        throw new InvalidOperationException("JWT Audience is not configured in appsettings.json.");
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidIssuer = issuer,
        ValidAudience = audience
    };
});
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("ViewerOnly", policy => policy.RequireRole("Viewer"));
            });
            #endregion

            builder.Logging.AddConsole();
            #region DbContext
            builder.Services.AddDbContext<ApplicationDbcontext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            #endregion
            #region Services Register
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();  
            builder.Services.AddScoped<IMetricsService, MetricsService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
            #endregion
            builder.Services.AddSwaggerGen();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddMemoryCache();

            var app = builder.Build();

            app.UseSwagger(); 
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;  
            });

            #region comprehensive middleware pipeline
            app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseHttpLogging();
            app.UseResponseCompression();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseIpRateLimiting();
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<AuditMiddleware>();
            app.MapHealthChecks("/health");
            app.UseStaticFiles();
            #endregion
            #region Swagger Documentaion
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            #endregion
            #region  Minimal API

            #region Login AND Register
            var authApiGroup = app.MapGroup("/api/auth");

            authApiGroup.MapPost("/login", async (IUserService userService, IJwtTokenService jwtTokenService, LoginDTO loginRequest) =>
            {
                var user = await userService.GetUserWithRoleByUserNameAsync(loginRequest.UserName);
                if (user == null || user.PasswordHash != loginRequest.Password) // Replace with proper password validation (e.g., hashed password)
                {
                    return Results.Json(new { message = "Invalid credentials" }, statusCode: StatusCodes.Status401Unauthorized);
                }

                var token = jwtTokenService.GenerateJwtToken(user);

                return Results.Ok(new AuthResponseDTO { Token = token });
            });
            authApiGroup.MapPost("/register", async (IUserService userService, RegisterDTO registerRequest) =>
            {
                var userExists = await userService.UserExistsAsync(registerRequest.UserName);
                if (userExists)
                {
                    return Results.Json(new { message = "Username already exists" }, statusCode: StatusCodes.Status400BadRequest);
                }

                var user = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = registerRequest.UserName,
                    PasswordHash = registerRequest.Password,
                    RoleId = registerRequest.RoleId
                };

                // 3. Save the new user
                await userService.CreateUserAsync(user);

                // 4. Return success response
                return Results.Ok(new { message = "User registered successfully" });
            });
            var roleApiGroup = app.MapGroup("/api/roles");

            roleApiGroup.MapPost("/add", async (IRoleService roleService, AddRoleDTO roleDto) =>
            {
                // Check if the role already exists
                if (await roleService.RoleExistsAsync(roleDto.RoleName))
                {
                    return Results.BadRequest(new { message = "Role already exists" });
                }

                // Create a new role entity
                var role = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = roleDto.RoleName
                };

                await roleService.AddRoleAsync(role);

                return Results.Ok(new { message = "Role added successfully", RoleId = role.Id });
            });
            #endregion
          
            #region Category

            var categoryApiGroup = app.MapGroup("/api/categories").RequireAuthorization();

            categoryApiGroup.MapGet("/", async (ICategoryService categoryService, HttpContext httpContext) =>
            {
                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    throw new UnauthorizedAccessException();
                }

                var categories = await categoryService.GetAllCategoriesAsync();
                return Results.Ok(categories.Select(c => new CategoryResponseDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ParentCategoryId = c.ParentCategoryId,
                    Status = c.Status.ToString(),
                    CreatedDate = c.CreatedDate,
                    UpdatedDate = c.UpdatedDate
                }));
            })
            .WithName("GetAllCategories")
            .WithMetadata(new EndpointNameMetadata("Get All Categories"));

            categoryApiGroup.MapGet("/{id:guid}", async (ICategoryService categoryService, Guid id, HttpContext httpContext) =>
            {
                try
                {
                    // Get subcategories and their products for the given parent category ID
                    var subcategoriesWithProducts = await categoryService.GetSubcategoriesAsync(id);
                    return Results.Ok(subcategoriesWithProducts);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            })
            .WithName("GetCategoryById")
             .WithMetadata(new EndpointNameMetadata("Get Category By Id"));

            categoryApiGroup.MapPost("/",
                [Authorize(Roles = "Admin")]
            async (ICategoryService categoryService, CategoryRequestDTO categoryDto) =>
                {
                    // Ensure the category name is unique before adding
                    var existingCategory = await categoryService.GetCategoryByNameAsync(categoryDto.Name);
                    if (existingCategory != null)
                        return Results.BadRequest(new { message = "Category name must be unique." });

                    var category = new Category
                    {
                        Name = categoryDto.Name,
                        Description = categoryDto.Description,
                        ParentCategoryId = categoryDto.ParentCategoryId,
                        Status = categoryDto.Status,
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow
                    };

                    await categoryService.AddCategoryAsync(category);
                    return Results.Created($"/api/categories/{category.Id}", category);
                })
            .WithName("CreateCategory")
            .WithMetadata(new EndpointNameMetadata("Create Category"));
            categoryApiGroup.MapPut("/{id:guid}",
                async (ICategoryService categoryService, Guid id, CategoryRequestDTO categoryDto) =>
                {
                    // Get the existing category
                    var category = await categoryService.GetCategoryByIdAsync(id);
                    if (category == null)
                        return Results.NotFound(new { message = "Category not found" });

                    // Check if another category with the same name exists, excluding the current category
                    var existingCategory = await categoryService.GetCategoryByNameAsync(categoryDto.Name);
                    if (existingCategory != null && existingCategory.Id != id)
                        return Results.BadRequest(new { message = "Category name must be unique" });

                    // Update category properties
                    category.Name = categoryDto.Name;
                    category.Description = categoryDto.Description;
                    category.ParentCategoryId = categoryDto.ParentCategoryId;
                    category.Status = categoryDto.Status;
                    category.UpdatedDate = DateTime.UtcNow;

                    // Save the updated category
                    await categoryService.UpdateCategoryAsync(category);

                    return Results.NoContent();
                })
                .WithName("UpdateCategory")
                .WithMetadata(new EndpointNameMetadata("Update Category"));


            categoryApiGroup.MapDelete("/{id:guid}",
         [Authorize(Roles = "Admin")]
            async (ICategoryService categoryService, Guid id, Guid? newCategoryId) =>
         {
             // Retrieve the category by the given id
             var category = await categoryService.GetCategoryByIdAsync(id);
             if (category == null)
                 return Results.NotFound(new { message = "Category not found" });

             if (newCategoryId.HasValue && newCategoryId.Value == id)
             {
                 return Results.BadRequest(new { message = "Cannot delete a category and assign it to itself" });
             }

             // Proceed with category deletion
             if (newCategoryId.HasValue)
             {
                 await categoryService.DeleteCategoryAsync(id, newCategoryId.Value);
             }
             else
             {
                 await categoryService.DeleteCategoryAsync(id);
             }

             // Return no content to indicate successful deletion
             return Results.NoContent();
         })
         .WithName("DeleteCategory")
         .WithMetadata(new EndpointNameMetadata("Delete Category"));

            #endregion

            #region Products
            var productApiGroup = app.MapGroup("/api/products");

            productApiGroup.MapGet("/",async (IProductService productService, int pageNumber, int pageSize) =>
            {
                var products = await productService.GetProductsAsync(pageNumber, pageSize);
                return Results.Ok(products.Select(p => new ProductResponseDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    CategoryId = p.CategoryId,
                    StockQuantity = p.StockQuantity,
                    ImageUrl = p.ImageUrl,
                    CreatedDate = p.CreatedDate,
                    UpdatedDate = p.UpdatedDate,
                    status = p.Status.ToString(),
                    CategoryName = p.Category.Name,  // Set the Category Name

                }));
            })
            .WithName("GetAllProducts")
            .WithMetadata(new EndpointNameMetadata("Get All Products"));

            productApiGroup.MapGet("/{id:guid}", async (IProductService productService, Guid id) =>
            {
                var product = await productService.GetProductByIdAsync(id);
                if (product == null)
                    return Results.NotFound(new { message = "Product not found" });


                return Results.Ok(new ProductResponseDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    CategoryId = product.CategoryId,
                    StockQuantity = product.StockQuantity,
                    ImageUrl = product.ImageUrl,
                    CreatedDate = product.CreatedDate,
                    UpdatedDate = product.UpdatedDate,
                    CategoryName = product.Category.Name,
                });
            })
            .WithName("GetProductById")
            .WithMetadata(new EndpointNameMetadata("Get Single Product"));


            productApiGroup.MapPost("/", [Authorize(Roles = "Admin")]
            async (IProductService productService, IFormFile imageFile,
            [FromForm] ProductRequestDTO productDto, IHostingEnvironment env) =>
            {
                // Validate the product data
                if (!Validator.TryValidateObject(productDto, new ValidationContext(productDto), null, true))
                    return Results.BadRequest(new { message = "Invalid product data" });

                // Check if an image file is provided
                if (imageFile == null || imageFile.Length == 0)
                    return Results.BadRequest(new { message = "Image file is required" });

                // Use the original file name (keep the extension)
                var fileName = imageFile.FileName;  // This keeps the original file name

                // Define the path inside the wwwroot/images directory
                var imagesDirectory = Path.Combine(env.WebRootPath, "images");

                // Ensure the images directory exists
                if (!Directory.Exists(imagesDirectory))
                {
                    Directory.CreateDirectory(imagesDirectory); // Create the directory if it doesn't exist
                }

                // Construct the file path within the images directory (use the original file name)
                var filePath = Path.Combine(imagesDirectory, fileName);

                // Check if the file already exists to avoid overwriting
                if (File.Exists(filePath))
                {
                    return Results.BadRequest(new { message = "File with the same name already exists" });
                }

                // Save the image file to disk
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                // Create the product object and set the ImageUrl to the relative URL
                var product = new Product
                {
                    Name = productDto.Name,
                    Description = productDto.Description,
                    Price = productDto.Price,
                    CategoryId = productDto.CategoryId,
                    StockQuantity = productDto.StockQuantity,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    ImageUrl = $"/images/{fileName}"
                };

                await productService.AddProductAsync(product);

                return Results.Created($"/api/products/{product.Id}", product);
            })
           .WithName("CreateProduct")
            .WithMetadata(new EndpointNameMetadata("Create New Product")).DisableAntiforgery();

            productApiGroup.MapPut("/{id:guid}", [Authorize(Roles = "Admin")]
            async (IProductService productService, Guid id, ProductRequestDTO productDto) =>
            {
                if (!Validator.TryValidateObject(productDto, new ValidationContext(productDto), null, true))
                    return Results.BadRequest(new { message = "Invalid product data" });

                var product = await productService.GetProductByIdAsync(id);
                if (product == null)
                    return Results.NotFound(new { message = "Product not found" });

                product.Name = productDto.Name;
                product.Description = productDto.Description;
                product.Price = productDto.Price;
                product.CategoryId = productDto.CategoryId;
                product.StockQuantity = productDto.StockQuantity;
                product.ImageUrl = productDto.Imageurl;
                product.UpdatedDate = DateTime.UtcNow;

                await productService.UpdateProductAsync(product);
                return Results.NoContent();
            })
            .WithName("UpdateProduct")
            .WithMetadata(new EndpointNameMetadata("Update Product"));

            productApiGroup.MapDelete("/{id:guid}", [Authorize(Roles = "Admin")]
            async (IProductService productService, Guid id) =>
            {
                var product = await productService.GetProductByIdAsync(id);
                if (product == null)
                    return Results.NotFound(new { message = "Product not found" });

                await productService.DeleteProductAsync(id);
                return Results.NoContent();
            })
            .WithName("DeleteProduct")
            .WithMetadata(new EndpointNameMetadata("Delete Product"));

            // Batch delete products
            productApiGroup.MapDelete("/batch", [Authorize(Roles = "Admin")]
            async (IProductService productService, [FromBody] IEnumerable<Guid> productIds) =>
            {
                if (productIds == null || !productIds.Any())
                    return Results.BadRequest(new { message = "Product IDs cannot be null or empty." });

                await productService.BatchDeleteAsync(productIds);
                return Results.NoContent();
            })
        .WithName("BatchDeleteProducts")
        .WithMetadata(new EndpointNameMetadata("Batch Delete Products"));


            #endregion

            #region Dashboard
            var dashboardApiGroup = app.MapGroup("/api/dashboard").RequireAuthorization() ;

            dashboardApiGroup.MapGet("/",
            async ([FromServices] IMetricsService metricsService) =>
            
            {
               


                var totalProducts = await metricsService.GetTotalProductsAsync();
                var totalCategories = await metricsService.GetTotalCategoriesAsync();
                var productsPerCategory = await metricsService.GetProductsPerCategoryAsync();
                var lowStockProducts = await metricsService.GetLowStockProductsAsync(10); 
                var recentActivities = await metricsService.GetRecentActivitiesAsync();

                var dashboardData = new
                {
                    TotalProducts = totalProducts,
                    TotalCategories = totalCategories,
                    ProductsPerCategory = productsPerCategory,
                    LowStockProducts = lowStockProducts,
                    RecentActivities = recentActivities
                };

                return Results.Ok(dashboardData);
            })
            .WithName("GetDashboardMetrics")
            .WithMetadata(new EndpointNameMetadata("Get Dashboard Metrics"));

            // Get products per category
            dashboardApiGroup.MapGet("/products-per-category", [Authorize(Roles = "Admin")]
            async ([FromServices] IMetricsService metricsService) =>
            {
                var productsPerCategory = await metricsService.GetProductsPerCategoryAsync();
                return Results.Ok(productsPerCategory);
            })
            .WithName("GetProductsPerCategory")
            .WithMetadata(new EndpointNameMetadata("Get Products Per Category"));

            // Get low stock products based on threshold
            dashboardApiGroup.MapGet("/low-stock/{threshold:int}",
                [Authorize(Roles = "Admin")] 
            async ([FromServices] IMetricsService metricsService, int threshold) =>
            {
                var lowStockProducts = await metricsService.GetLowStockProductsAsync(threshold);
                return Results.Ok(lowStockProducts);
            })
            .WithName("GetLowStockProducts")
            .WithMetadata(new EndpointNameMetadata("Get Low Stock Products"));

            // Get recent activities
            dashboardApiGroup.MapGet("/recent-activities", [Authorize(Roles = "Admin")]
            async ([FromServices] IMetricsService metricsService) =>
            {
                var recentActivities = await metricsService.GetRecentActivitiesAsync();
                return Results.Ok(recentActivities);
            })
            .WithName("GetRecentActivities")
            .WithMetadata(new EndpointNameMetadata("Get Recent Activities"));
            #endregion

            #endregion

            app.Run();
        }
    }
}
