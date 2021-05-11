namespace DeviceSimulator.Web
{
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Schema.Mutation;
using Schema.Query;
using DeviceSimulator;
using HotChocolate.Subscriptions;
	public class Startup{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			var iothubConnectionString = Environment.GetEnvironmentVariable("IOTHUB_CONNECTION_STRING");	
			services.AddSingleton<IDeviceFactory>( (sp) => new IotHubDeviceFactory(iothubConnectionString));
			services.AddSingleton<IDeviceRegistrar>( (sp) => new IotHubDeviceRegistrar(iothubConnectionString));
			services.AddSingleton<IDeviceManager, OnmemoryDeviceManager>();

			services
				.AddGraphQLServer()
				.AddQueryType<Query>()
				.AddMutationType<Mutation>();

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting().UseEndpoints(endpoints =>
				   {
					   endpoints.MapGraphQL();
				   });
		}
    }
}