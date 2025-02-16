﻿using System;
using System.Threading.Tasks;
using CareerConnect.Infrastructure.DbContexts;
using CareerConnect.Infrastructure.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CareerConnect.Test.UnitTest
{
    public class JobTest
    {
        [Fact]
        public async Task CreateJob()
        {
            var serviceProvider = BuildServiceProvider();
            var jobService = serviceProvider.GetRequiredService<JobService>();
            var result = await jobService.CreateJobAsync("Test", "Address","Description", 10, 20, DateTime.Now);
            Assert.True(result.Success);
        }


        [Fact]
        public async Task CreateApplicant()
        {
            var serviceProvider = BuildServiceProvider();
            var jobService = serviceProvider.GetRequiredService<JobService>();
            var result = await jobService.CreateJobAsync("Test","Address" ,"Description", 10, 20, DateTime.Now);
            Assert.True(result.Success);
        }


        private static IServiceProvider BuildServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddHttpContextAccessor();
            services.AddScoped<UserInformation>();
            services.AddScoped<JobService>();
            services.AddScoped<AuthenticationService>();
            services.AddDbContext<CoreDbContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()));
            services.AddDbContext<AccountDbContext>(b => b.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services.AddIdentityCore<IdentityUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.User.RequireUniqueEmail = true;
                    options.Lockout.MaxFailedAccessAttempts = 10;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(60);
                }).AddEntityFrameworkStores<AccountDbContext>()
                .AddUserManager<UserManager<IdentityUser>>()
                .AddSignInManager();

            services.AddSession(options =>
            {
                options.Cookie.Name = ".career.connect.session";
            });

            services.AddAuthentication()
                .AddCookie(options =>
                {
                    options.Cookie.Name = ".career.connect.cookie";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                    options.SlidingExpiration = true;
                    options.LoginPath = "/Account/LogIn";
                });

            services.AddLogging();

            return services.BuildServiceProvider();
        }
    }
}
