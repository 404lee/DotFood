using Microsoft.AspNetCore.Mvc.Testing;
using SolrNet;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using DotFood;
using Xunit.Abstractions;
using System.Text.RegularExpressions;
using DotFood.Entity;
using Microsoft.AspNetCore.Identity;
using DotFood.Controllers;
using DotFood.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using DotFood.Data;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.EntityFrameworkCore;

namespace DotFoodTest
{
    public class UnitTest1 : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        private readonly UserManager<Users> _userManager;
        private readonly WebApplicationFactory<Program> _factory;



        public UnitTest1(WebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _output = output;
        }
        //    [Fact]
        public void Test1()
        {
            Assert.True(true);


        }

        #region Login

        [Fact]
        public async Task Login_WithValidCustomerCredential_ShouldRedirectToDashboard()
        {
            var getResponse = await _client.GetAsync("/Account/Login");
            var getContent = await getResponse.Content.ReadAsStringAsync();

            var tokenMatch = Regex.Match(getContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");
            var token = tokenMatch.Groups[1].Value;

            var formData = new Dictionary<string, string>
    {
        { "Email", "test1@test.com" },
        { "Password", "Test1234$" },
        { "RememberMe", "true" },
        { "__RequestVerificationToken", token }
    };

            var content = new FormUrlEncodedContent(formData);
            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/Login")
            {
                Content = content
            };

            postRequest.Headers.Referrer = new Uri("http://localhost/Account/Login");
            var response = await _client.SendAsync(postRequest);


            var responseHtml = await response.Content.ReadAsStringAsync();

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response);
            Assert.NotNull(responseContent);
            Assert.Contains("Customer", responseHtml);
            _output.WriteLine($"Response Content: {responseContent}");
        }


        [Fact]
        public async Task Login_WithValidVendorCredential_ShouldRedirectToDashboard()
        {
            var getResponse = await _client.GetAsync("/Account/Login");
            var getContent = await getResponse.Content.ReadAsStringAsync();

            var tokenMatch = Regex.Match(getContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");
            var token = tokenMatch.Groups[1].Value;

            var formData = new Dictionary<string, string>
    {
        { "Email", "vendor@test.com" },
        { "Password", "Vendor123#" },
        { "RememberMe", "true" },
        { "__RequestVerificationToken", token }
    };

            var content = new FormUrlEncodedContent(formData);
            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/Login")
            {
                Content = content
            };

            postRequest.Headers.Referrer = new Uri("http://localhost/Account/Login");

            var response = await _client.SendAsync(postRequest);
            var responseHtml = await response.Content.ReadAsStringAsync();

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response);
            Assert.NotNull(responseContent);
            Assert.Contains("Vendor", responseHtml);
            _output.WriteLine($"Response Content: {responseContent}");
        }

        [Fact]
        public async Task Login_WithInValidCustomerCredential_ShouldRedirectToDashboard()
        {
            var getResponse = await _client.GetAsync("/Account/Login");
            var getContent = await getResponse.Content.ReadAsStringAsync();

            var tokenMatch = Regex.Match(getContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");
            var token = tokenMatch.Groups[1].Value;

            var formData = new Dictionary<string, string>
    {
        { "Email", "test11@test.com" },
        { "Password", "T1234$" },
        { "RememberMe", "true" },
        { "__RequestVerificationToken", token }
    };

            var content = new FormUrlEncodedContent(formData);
            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/Login")
            {
                Content = content
            };

            postRequest.Headers.Referrer = new Uri("http://localhost/Account/Login");

            var response = await _client.SendAsync(postRequest);
            var responseHtml = await response.Content.ReadAsStringAsync();

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response);
            Assert.NotNull(responseContent);
            Assert.Contains("Login", responseHtml);
            _output.WriteLine($"Response Content: {responseContent}");
        }
        [Fact]
        public async Task Login_Withsql_ShouldRedirectToDashboard()
        {
            var getResponse = await _client.GetAsync("/Account/Login");
            var getContent = await getResponse.Content.ReadAsStringAsync();

            var tokenMatch = Regex.Match(getContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");
            var token = tokenMatch.Groups[1].Value;

            var formData = new Dictionary<string, string>
    {
        { "Email", "test11@test.com'--" },
        { "Password", "T12345$" },
        { "RememberMe", "true" },
        { "__RequestVerificationToken", token }
    };

            var content = new FormUrlEncodedContent(formData);
            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/Login")
            {
                Content = content
            };

            postRequest.Headers.Referrer = new Uri("http://localhost/Account/Login");

            var response = await _client.SendAsync(postRequest);
            var responseHtml = await response.Content.ReadAsStringAsync();

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response);
            Assert.NotNull(responseContent);
            Assert.Contains("Login", responseHtml);
            _output.WriteLine($"Response Content: {responseContent}");
        }


        #endregion



        #region Register 


        [Fact]
        public async Task Register_WithValidData_ShouldCreateUser_CorrectlyAndPersistInDatabase()
        {
            // Arrange - Get AntiForgery Token
            var getResponse = await _client.GetAsync("/Account/Register");
            var getContent = await getResponse.Content.ReadAsStringAsync();

            var tokenMatch = Regex.Match(getContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");
            var token = tokenMatch.Success ? tokenMatch.Groups[1].Value : string.Empty;

            var uniqueEmail = $"testuser_{Guid.NewGuid():N}@test.com";

            var formData = new Dictionary<string, string>
    {
        { "FullName", "newuser11" },
        { "Email", "newuser11@test.com" },
        { "Password", "Test1234$" },
        { "ConfirmPassword", "Test1234$" },
        { "Country", "Jordan" },
        { "City", "Amman" },
        { "Role", "customer" },
        { "__RequestVerificationToken", token }
    };

            var content = new FormUrlEncodedContent(formData);

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/Register")
            {
                Content = content
            };
            postRequest.Headers.Referrer = new Uri("http://localhost/Account/Register");

            var response = await _client.SendAsync(postRequest);

        


            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UsersContext>();

                var user = await dbContext.Users.FirstOrDefaultAsync(u =>
                    u.FullName == "newuser11" &&
                    u.Country == "Jordan" &&
                    u.City == "Amman");
                Assert.NotNull(user);
            }
        }

        [Fact]
        public async Task Register_WithInValidData_WeakPass_ShouldnotCreateUser()
        {
            // Arrange - Get AntiForgery Token
            var getResponse = await _client.GetAsync("/Account/Register");
            var getContent = await getResponse.Content.ReadAsStringAsync();

            var tokenMatch = Regex.Match(getContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");
            var token = tokenMatch.Success ? tokenMatch.Groups[1].Value : string.Empty;

            var uniqueEmail = $"testuser_{Guid.NewGuid():N}@test.com";

            var formData = new Dictionary<string, string>
    {
        { "FullName", "newuser1199" },
        { "Email", "newuser11@test.com" },
        { "Password", "Test12349" },
        { "ConfirmPassword", "Test12349" },
        { "Country", "Jordan" },
        { "City", "Amman" },
        { "Role", "customer" },
        { "__RequestVerificationToken", token }
    };

            var content = new FormUrlEncodedContent(formData);

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/Register")
            {
                Content = content
            };
            postRequest.Headers.Referrer = new Uri("http://localhost/Account/Register");

            var response = await _client.SendAsync(postRequest);




            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UsersContext>();

                var user = await dbContext.Users.FirstOrDefaultAsync(u =>
                    u.FullName == "newuser1199" &&
                    u.Country == "Jordan" &&
                    u.City == "Amman");
                Assert.NotNull(user);
            }
        }

        [Fact]
        public async Task Register_WithInValidData_ShortName_ShouldnotCreateUser()
        {
            // Arrange - Get AntiForgery Token
            var getResponse = await _client.GetAsync("/Account/Register");
            var getContent = await getResponse.Content.ReadAsStringAsync();

            var tokenMatch = Regex.Match(getContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");
            var token = tokenMatch.Success ? tokenMatch.Groups[1].Value : string.Empty;

            var uniqueEmail = $"testuser_{Guid.NewGuid():N}@test.com";

            var formData = new Dictionary<string, string>
    {
        { "FullName", "newuser" },
        { "Email", "newuser11@test.com" },
        { "Password", "Test12349" },
        { "ConfirmPassword", "Test12349" },
        { "Country", "Jordan" },
        { "City", "Amman" },
        { "Role", "customer" },
        { "__RequestVerificationToken", token }
    };

            var content = new FormUrlEncodedContent(formData);

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/Register")
            {
                Content = content
            };
            postRequest.Headers.Referrer = new Uri("http://localhost/Account/Register");

            var response = await _client.SendAsync(postRequest);




            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UsersContext>();

                var user = await dbContext.Users.FirstOrDefaultAsync(u =>
                    u.FullName == "newuser" &&
                    u.Country == "Jordan" &&
                    u.City == "Amman");
                Assert.NotNull(user);
            }
        }


        [Fact]
        public async Task Register_WithInValidData_notfoundrole_ShouldnotCreateUser()
        {
            // Arrange - Get AntiForgery Token
            var getResponse = await _client.GetAsync("/Account/Register");
            var getContent = await getResponse.Content.ReadAsStringAsync();

            var tokenMatch = Regex.Match(getContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");
            var token = tokenMatch.Success ? tokenMatch.Groups[1].Value : string.Empty;

            var uniqueEmail = $"testuser_{Guid.NewGuid():N}@test.com";

            var formData = new Dictionary<string, string>
    {
        { "FullName", "newuseradmin" },
        { "Email", "newuser11@test.com" },
        { "Password", "Test1234$" },
        { "ConfirmPassword", "Test1234$" },
        { "Country", "Jordan" },
        { "City", "Amman" },
        { "Role", "Admin" },
        { "__RequestVerificationToken", token }
    };

            var content = new FormUrlEncodedContent(formData);

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/Register")
            {
                Content = content
            };
            postRequest.Headers.Referrer = new Uri("http://localhost/Account/Register");

            var response = await _client.SendAsync(postRequest);




            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UsersContext>();

                var user = await dbContext.Users.FirstOrDefaultAsync(u =>
                    u.FullName == "newuseradmin" &&
                    u.Country == "Jordan" &&
                    u.City == "Amman");
                Assert.NotNull(user);
            }
        }

        #endregion




        #region  UpdateProfile

       

        [Fact]
        public async Task UpdateProfile_WithValidData_ShouldUpdateSuccessfullyAndPersistInDatabase()
        {
            // Arrange: Login first
            var loginResponse = await _client.GetAsync("/Account/Login");
            var loginContent = await loginResponse.Content.ReadAsStringAsync();

            var loginTokenMatch = Regex.Match(loginContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");

            var loginToken = loginTokenMatch.Success ? loginTokenMatch.Groups[1].Value : string.Empty;

            var loginFormData = new Dictionary<string, string>
    {
        { "Email", "unittest101@test.com" }, 
        { "Password", "Unittest1234$" },
        { "RememberMe", "true" },
        { "__RequestVerificationToken", loginToken }
    };

            var loginContentPost = new FormUrlEncodedContent(loginFormData);
            var loginPostRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/Login")
            {
                Content = loginContentPost
            };
            loginPostRequest.Headers.Referrer = new Uri("http://localhost/Account/Login");

            await _client.SendAsync(loginPostRequest);

            var getProfileResponse = await _client.GetAsync("/Account/EditProfile");
            var getProfileContent = await getProfileResponse.Content.ReadAsStringAsync();

            var profileTokenMatch = Regex.Match(getProfileContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");

            var profileToken = profileTokenMatch.Success ? profileTokenMatch.Groups[1].Value : string.Empty;

            var updateFormData = new Dictionary<string, string>
    {
        { "Name", "manaralqassas" },
        { "Country", "Jordan" },
        { "City", "Amman" },
        { "__RequestVerificationToken", profileToken }
    };

            var updateContent = new FormUrlEncodedContent(updateFormData);
            var updatePostRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/UpdateProfile")
            {
                Content = updateContent
            };
            updatePostRequest.Headers.Referrer = new Uri("http://localhost/Account/EditProfile");

            var updateResponse = await _client.SendAsync(updatePostRequest);

            Assert.True(updateResponse.StatusCode == HttpStatusCode.OK || updateResponse.StatusCode == HttpStatusCode.Redirect);




            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UsersContext>();

                var user = await dbContext.Users.FirstOrDefaultAsync(u =>
                    u.Email == "unittest101@test.com" &&
                    u.FullName == "manar" &&
                    u.Country == "Jordan" &&
                    u.City == "Amman");
                Assert.NotNull(user);
            }






        }

        [Fact]
        public async Task UpdateProfile_WithInValidData_ShouldNotUpdated()
        {
            // Arrange: Login first
            var loginResponse = await _client.GetAsync("/Account/Login");
            var loginContent = await loginResponse.Content.ReadAsStringAsync();

            var loginTokenMatch = Regex.Match(loginContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");

            var loginToken = loginTokenMatch.Success ? loginTokenMatch.Groups[1].Value : string.Empty;

            var loginFormData = new Dictionary<string, string>
    {
        { "Email", "unittest101@test.com" },
        { "Password", "Unittest1234$" },
        { "RememberMe", "true" },
        { "__RequestVerificationToken", loginToken }
    };

            var loginContentPost = new FormUrlEncodedContent(loginFormData);
            var loginPostRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/Login")
            {
                Content = loginContentPost
            };
            loginPostRequest.Headers.Referrer = new Uri("http://localhost/Account/Login");

            await _client.SendAsync(loginPostRequest);

            var getProfileResponse = await _client.GetAsync("/Account/EditProfile");
            var getProfileContent = await getProfileResponse.Content.ReadAsStringAsync();

            var profileTokenMatch = Regex.Match(getProfileContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");

            var profileToken = profileTokenMatch.Success ? profileTokenMatch.Groups[1].Value : string.Empty;

            var updateFormData = new Dictionary<string, string>
    {
        { "Name", "manar" },
        { "Country", "Jordan" },
        { "City", "Amman" },
        { "__RequestVerificationToken", profileToken }
    };

            var updateContent = new FormUrlEncodedContent(updateFormData);
            var updatePostRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/UpdateProfile")
            {
                Content = updateContent
            };
            updatePostRequest.Headers.Referrer = new Uri("http://localhost/Account/EditProfile");

            var updateResponse = await _client.SendAsync(updatePostRequest);

            Assert.True(updateResponse.StatusCode == HttpStatusCode.OK || updateResponse.StatusCode == HttpStatusCode.Redirect);


            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UsersContext>();

                var user = await dbContext.Users.FirstOrDefaultAsync(u =>
                    u.Email == "unittest101@test.com" &&
                    u.FullName == "manar" &&
                    u.Country == "Jordan" &&
                    u.City == "Amman");
                Assert.NotNull(user);
            }



        }


        [Fact]
        public async Task UpdateProfile_WithInValidData_ShouldNotUpdate()
        {
            // Arrange: Login first
            var loginResponse = await _client.GetAsync("/Account/Login");
            var loginContent = await loginResponse.Content.ReadAsStringAsync();

            var loginTokenMatch = Regex.Match(loginContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");

            var loginToken = loginTokenMatch.Success ? loginTokenMatch.Groups[1].Value : string.Empty;

            var loginFormData = new Dictionary<string, string>
    {
        { "Email", "test1@test.com" }, // user already exists
        { "Password", "Test1234$" },
        { "RememberMe", "true" },
        { "__RequestVerificationToken", loginToken }
    };

            var loginContentPost = new FormUrlEncodedContent(loginFormData);
            var loginPostRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/Login")
            {
                Content = loginContentPost
            };
            loginPostRequest.Headers.Referrer = new Uri("http://localhost/Account/Login");

            await _client.SendAsync(loginPostRequest);

            var getProfileResponse = await _client.GetAsync("/Account/EditProfile");
            var getProfileContent = await getProfileResponse.Content.ReadAsStringAsync();

            var profileTokenMatch = Regex.Match(getProfileContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");

            var profileToken = profileTokenMatch.Success ? profileTokenMatch.Groups[1].Value : string.Empty;

            var updateFormData = new Dictionary<string, string>
    {
        { "Name", "updateduserrrrrr" },
        { "Country", "UpdatedCountry" },
        { "City", "Amman" },
        { "__RequestVerificationToken", profileToken }
    };

            var updateContent = new FormUrlEncodedContent(updateFormData);
            var updatePostRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/UpdateProfile")
            {
                Content = updateContent
            };
            updatePostRequest.Headers.Referrer = new Uri("http://localhost/Account/EditProfile");

            var updateResponse = await _client.SendAsync(updatePostRequest);

            Assert.True(updateResponse.StatusCode == HttpStatusCode.OK || updateResponse.StatusCode == HttpStatusCode.Redirect);


            _output.WriteLine("Profile updated successfully.");


            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UsersContext>();

                var user = await dbContext.Users.FirstOrDefaultAsync(u =>
                    u.Email == "unittest101@test.com" &&
                    u.FullName == "manar" &&
                    u.Country == "Jordan" &&
                    u.City == "Amman");
                Assert.NotNull(user);
            }






        }




        #endregion




        #region add item
        [Fact]
        public async Task AddItem_WithValidData_ShouldAddProductToDatabase()
        {
            // Arrange: Login as Vendor
            var loginResponse = await _client.GetAsync("/Account/Login");
            var loginContent = await loginResponse.Content.ReadAsStringAsync();


            var loginTokenMatch = Regex.Match(loginContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");

            var loginToken = loginTokenMatch.Success ? loginTokenMatch.Groups[1].Value : string.Empty;

            var loginFormData = new Dictionary<string, string>
    {
        { "Email", "vendortest@test.com" },
        { "Password", "Vendor1234$" },
        { "RememberMe", "true" },
        { "__RequestVerificationToken", loginToken }
    };

            var loginContentPost = new FormUrlEncodedContent(loginFormData);
            var loginPostRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/Login")
            {
                Content = loginContentPost
            };
            loginPostRequest.Headers.Referrer = new Uri("http://localhost/Account/Login");

            await _client.SendAsync(loginPostRequest);

            // Arrange: Get the AddItem page and extract AntiForgery token
            var getAddItemResponse = await _client.GetAsync("/Vendor/AddItem");
            var getAddItemContent = await getAddItemResponse.Content.ReadAsStringAsync();
            var addItemTokenMatch = Regex.Match(getAddItemContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");


            var addItemToken = addItemTokenMatch.Success ? addItemTokenMatch.Groups[1].Value : string.Empty;


            var formData = new Dictionary<string, string>
    {
        { "Name", "UnitTestProduct0088" },
        { "CategoryId", "1" }, 
        { "Description", "Product created during unit test" },
        { "Price", "16.99" },
        { "Quantity", "10" },
        { "__RequestVerificationToken", addItemToken }
    };

            var content = new FormUrlEncodedContent(formData);

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Vendor/AddItem")
            {
                Content = content
            };
            postRequest.Headers.Referrer = new Uri("http://localhost/Vendor/AddItem");

            // Act: Send POST request
            var postResponse = await _client.SendAsync(postRequest);

            // Assert: Check Response
            Assert.NotNull(postResponse);


            // Assert: Check if product exists in DB
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UsersContext>();

                var product = await dbContext.Products.FirstOrDefaultAsync(p =>
                    p.Name == "UnitTestProduct0088" &&
                    p.Description == "Product created during unit test" &&
                    p.Price == 16.99m &&
                    p.Quantity == 10 &&
                    p.CategoryId == 1);
                Assert.NotNull(product);
                Assert.Equal(1, product.CategoryId); // Matching what was posted
                Assert.Equal("Product created during unit test", product.Description);
            }
        }
        [Fact]
        public async Task AddItem_WithInValidData_notfoundcategoryID_ShouldAddProductToDatabase()
        {
            // Arrange: Login as Vendor
            var loginResponse = await _client.GetAsync("/Account/Login");
            var loginContent = await loginResponse.Content.ReadAsStringAsync();


            var loginTokenMatch = Regex.Match(loginContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");

            var loginToken = loginTokenMatch.Success ? loginTokenMatch.Groups[1].Value : string.Empty;

            var loginFormData = new Dictionary<string, string>
    {
        { "Email", "vendortest2@test.com" },
        { "Password", "Vendor1234$" },
        { "RememberMe", "true" },
        { "__RequestVerificationToken", loginToken }
    };

            var loginContentPost = new FormUrlEncodedContent(loginFormData);
            var loginPostRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/Login")
            {
                Content = loginContentPost
            };
            loginPostRequest.Headers.Referrer = new Uri("http://localhost/Account/Login");

            await _client.SendAsync(loginPostRequest);

            // Arrange: Get the AddItem page and extract AntiForgery token
            var getAddItemResponse = await _client.GetAsync("/Vendor/AddItem");
            var getAddItemContent = await getAddItemResponse.Content.ReadAsStringAsync();
            var addItemTokenMatch = Regex.Match(getAddItemContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");


            var addItemToken = addItemTokenMatch.Success ? addItemTokenMatch.Groups[1].Value : string.Empty;

            // Prepare form data for new product
            var formData = new Dictionary<string, string>
    {
        { "Name", "UnitTestProduct3" },
        { "CategoryId", "5" }, // Assumed CategoryId 1 exists (Food for example)
        { "Description", "Product created during unit test" },
        { "Price", "16.99" },
        { "Quantity", "10" },
        { "__RequestVerificationToken", addItemToken }
    };

            var content = new FormUrlEncodedContent(formData);

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Vendor/AddItem")
            {
                Content = content
            };
            postRequest.Headers.Referrer = new Uri("http://localhost/Vendor/AddItem");

            // Act: Send POST request
            var postResponse = await _client.SendAsync(postRequest);

            // Assert: Check Response
            Assert.NotNull(postResponse);


            // Assert: Check if product exists in DB
     
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UsersContext>();

                var product = await dbContext.Products.FirstOrDefaultAsync(p =>
                    p.Name == "UnitTestProduct99" &&
                    p.Description == "Product created during unit test" &&
                    p.Price == 16.99m &&
                    p.Quantity == 10 &&
                    p.CategoryId == 5);
                Assert.NotNull(product);
                Assert.Equal(1, product.CategoryId); // Matching what was posted
                Assert.Equal("Product created during unit test", product.Description);
            }
        }
        
        [Fact]
        public async Task AddItems_WithInValidData_negativePrice_ShouldAddProductToDatabase()
        {
            var loginResponse = await _client.GetAsync("/Account/Login");
            var loginContent = await loginResponse.Content.ReadAsStringAsync();

            var loginTokenMatch = Regex.Match(loginContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");
            var loginToken = loginTokenMatch.Success ? loginTokenMatch.Groups[1].Value : string.Empty;

            var loginFormData = new Dictionary<string, string>
    {
        { "Email", "vendortest2@test.com" },
        { "Password", "Vendor1234$" },
        { "RememberMe", "true" },
        { "__RequestVerificationToken", loginToken }
    };

            var loginContentPost = new FormUrlEncodedContent(loginFormData);
            var loginPostRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/Login")
            {
                Content = loginContentPost
            };
            loginPostRequest.Headers.Referrer = new Uri("http://localhost/Account/Login");

            await _client.SendAsync(loginPostRequest);

            var getAddItemResponse = await _client.GetAsync("/Vendor/AddItem");
            var getAddItemContent = await getAddItemResponse.Content.ReadAsStringAsync();
            var addItemTokenMatch = Regex.Match(getAddItemContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");
            var addItemToken = addItemTokenMatch.Success ? addItemTokenMatch.Groups[1].Value : string.Empty;

            var formData = new Dictionary<string, string>
    {
        { "Name", "UnitTestProduct99" },
        { "CategoryId", "1" }, 
        { "Description", "Product created during unit test" },
        { "Price", "-16.99" },
        { "Quantity", "10" },
        { "__RequestVerificationToken", addItemToken }
    };

            var content = new FormUrlEncodedContent(formData);

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Vendor/AddItem")
            {
                Content = content
            };
            postRequest.Headers.Referrer = new Uri("http://localhost/Vendor/AddItem");

            var postResponse = await _client.SendAsync(postRequest);

            Assert.NotNull(postResponse);

            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UsersContext>();

                var product = await dbContext.Products.FirstOrDefaultAsync(p =>
                    p.Name == "UnitTestProduct99" &&
                    p.Description == "Product created during unit test" &&
                    p.Price == -16.99m &&
                    p.Quantity == 10);

                Assert.NotNull(product);
                Assert.Equal(1, product.CategoryId); 
                Assert.Equal("Product created during unit test", product.Description);
            }
        }
        [Fact]
        public async Task AddItems_WithInValidData_negativeQuantity_ShouldAddProductToDatabase()
        {
            // Arrange: Login as Vendor
            var loginResponse = await _client.GetAsync("/Account/Login");
            var loginContent = await loginResponse.Content.ReadAsStringAsync();

            var loginTokenMatch = Regex.Match(loginContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");
            var loginToken = loginTokenMatch.Success ? loginTokenMatch.Groups[1].Value : string.Empty;

            var loginFormData = new Dictionary<string, string>
    {
        { "Email", "vendortest2@test.com" },
        { "Password", "Vendor1234$" },
        { "RememberMe", "true" },
        { "__RequestVerificationToken", loginToken }
    };

            var loginContentPost = new FormUrlEncodedContent(loginFormData);
            var loginPostRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/Login")
            {
                Content = loginContentPost
            };
            loginPostRequest.Headers.Referrer = new Uri("http://localhost/Account/Login");

            await _client.SendAsync(loginPostRequest);

            // Arrange: Get the AddItem page and extract AntiForgery token
            var getAddItemResponse = await _client.GetAsync("/Vendor/AddItem");
            var getAddItemContent = await getAddItemResponse.Content.ReadAsStringAsync();
            var addItemTokenMatch = Regex.Match(getAddItemContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");
            var addItemToken = addItemTokenMatch.Success ? addItemTokenMatch.Groups[1].Value : string.Empty;

            // Prepare form data for new product
            var formData = new Dictionary<string, string>
    {
        { "Name", "invalidproduct" },
        { "CategoryId", "1" }, // Assumed CategoryId 1 exists
        { "Description", "Product created during unit test" },
        { "Price", "15" },
        { "Quantity", "-10" },
        { "__RequestVerificationToken", addItemToken }
    };

            var content = new FormUrlEncodedContent(formData);

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Vendor/AddItem")
            {
                Content = content
            };
            postRequest.Headers.Referrer = new Uri("http://localhost/Vendor/AddItem");

            // Act: Send POST request
            var postResponse = await _client.SendAsync(postRequest);

            // Assert: Check Response
            Assert.NotNull(postResponse);

            // Assert: Check if product exists in DB with more precise matching
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UsersContext>();

                var product = await dbContext.Products.FirstOrDefaultAsync(p =>
                    p.Name == "invalidproduct" &&
                    p.Description == "Product created during unit test" &&
                    p.Price == 15 &&
                    p.Quantity == -10);

                Assert.NotNull(product);
                Assert.Equal(1, product.CategoryId); // Matching what was posted
                Assert.Equal("Product created during unit test", product.Description);
            }
        }

        #endregion







        #region AddToCart 
        [Fact]
        public async Task AddToCart_WithExistingProduct_ShouldAddItemToCart()
        {
            // Arrange: Login as Customer
            var loginResponse = await _client.GetAsync("/Account/Login");
            var loginContent = await loginResponse.Content.ReadAsStringAsync();

            var loginTokenMatch = Regex.Match(loginContent, @"name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)""");

            var loginToken = loginTokenMatch.Success ? loginTokenMatch.Groups[1].Value : string.Empty;

            var loginFormData = new Dictionary<string, string>
    {
        { "Email", "customerunittest@test.com" },
        { "Password", "Test1234$" },
        { "RememberMe", "true" },
       { "__RequestVerificationToken", loginToken }
    };

            var loginContentPost = new FormUrlEncodedContent(loginFormData);
            var loginPostRequest = new HttpRequestMessage(HttpMethod.Post, "/Account/Login")
            {
                Content = loginContentPost
            };
            loginPostRequest.Headers.Referrer = new Uri("http://localhost/Account/Login");

            await _client.SendAsync(loginPostRequest);

            long existingProductId;

            // Arrange: Get existing Product from DB
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UsersContext>();

                var existingProduct = await dbContext.Products
                    .FirstOrDefaultAsync(p => p.Name == "UnitTestProduct"); // 

                Assert.NotNull(existingProduct); 

                existingProductId = existingProduct.Id;
            }

            // Act: POST to AddToCart
            var formData = new Dictionary<string, string>
    {
        { "productId", existingProductId.ToString() }
    };
            var postContent = new FormUrlEncodedContent(formData);

            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Customer/AddToCart")
            {
                Content = postContent
            };
            postRequest.Headers.Referrer = new Uri("http://localhost/Customer/Index");

            var postResponse = await _client.SendAsync(postRequest);

            // Assert
            Assert.NotNull(postResponse);
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<UsersContext>();

                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == "customerunittest@test.com");

                var cartItem = await dbContext.Cart.FirstOrDefaultAsync(c => c.CustomerId == user.Id && c.ProductId == existingProductId);

                Assert.NotNull(cartItem); // 
            }
        }

        #endregion




    }
}


