using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Workbench;

namespace Workbench
{
    public class MyConfigurationSource : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new MyConfigurationProvider(this);
        }
    }

    public class MyConfigurationProvider : ConfigurationProvider
    {
        private int time = 0;
        public MyConfigurationProvider(MyConfigurationSource source)
        {
            Source = source;
            
            var state = new object();
            var stateTimer = new Timer((obj) => { this.Load(); this.OnReload(); }, state, 1000, 2500);
            //stateTimer.
        }

        public MyConfigurationSource Source { get; }
        
        public override void Load()
        {
            base.Load();
            if (time == 0)
            {
                Set("Test1", "Value1");
                time = 1;
                
                return;
            }

            if (time == 1)
            {
                Set("Test1", "Value2");
            }
        }
        
        
    }
    
    public static class MyConfigurationExtensions
    {
        public static IConfigurationBuilder AddMyConfiguration(this IConfigurationBuilder configuration)
        {
            configuration.Add(new MyConfigurationSource());
            return configuration;
        }
    }
}