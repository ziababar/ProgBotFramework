﻿using Autofac;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System.Configuration;
using System.Web.Http;

namespace MusicChatbot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            Conversation.UpdateContainer(builder =>
            {
                var store = new TableBotDataStore(
                    ConfigurationManager
                        .ConnectionStrings["StorageConnectionString"]
                        .ConnectionString);

                builder.Register(c => store)
                    .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                    .AsSelf()
                    .SingleInstance();

                builder.Register(c => new CachingBotDataStore(store,
                    CachingBotDataStoreConsistencyPolicy
                    .ETagBasedConsistency))
                    .As<IBotDataStore<BotData>>()
                    .AsSelf()
                    .InstancePerLifetimeScope();
            });
        }
    }
}
