// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="AraCom IT Services AG">
// Copyright (c) AraCom IT Services AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TodoWebProjekt
{
    /// <summary>
    /// The Programm class, we starte our apllication with this class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The Main methode where we define the Host for our application.
        /// </summary>
        /// <param name="args"> arguments!?. </param>
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            host.Run();
        }

        /// <summary>
        /// Configure the host.
        /// </summary>
        /// <param name="args"> any arguments. </param>
        /// <returns> IHostBuilder. </returns>
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
        }
    }
}