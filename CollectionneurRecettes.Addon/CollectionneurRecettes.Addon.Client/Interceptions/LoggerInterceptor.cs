using Microsoft.Practices.Unity.InterceptionExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Client.Interceptions
{
    public class LoggerInterceptor : IInterceptionBehavior
    {
        public CollectionneurRecettes.Addon.CrossCutting.ILoggerService LoggerService { get; set; }

        public bool WillExecute
        {
            get
            {
                return true;
            }
        }

        public IEnumerable<Type> GetRequiredInterfaces()
        {
            return Type.EmptyTypes;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextInterceptionBehaviorDelegate getNext)
        {
            this.LoggerService.LogVerbose(string.Format("Start method: {0} with params {1}", input.MethodBase.ToString(), string.Empty));
            var result = getNext()(input, getNext);

            if (result.Exception != null)
            {
                this.LoggerService.LogVerbose(result.Exception.ToString());
            }

            return result;
        }
    }
}
