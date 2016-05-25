﻿using System;

namespace Nancy.OAuth2
{
    public static class OAuth
    {
        public static OAuthConfiguration Configuration = new OAuthConfiguration();

        public static bool IsEnabled { get; private set; }

        public static void Enable()
        {
            IsEnabled = true;
        }

        public static void Enable(Action<OAuthConfiguration> closure)
        {
            var configuration = new OAuthConfiguration();

            closure.Invoke(configuration);

            Enable(configuration);
        }

        public static void Enable(OAuthConfiguration configuration)
        {
            Configuration = configuration;
            IsEnabled = true;
        }
    }
}
