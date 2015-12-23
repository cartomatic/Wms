using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cartomatic.Wms
{
    public abstract partial class WmsDriver
    {
        /// <summary>
        /// Tries to work out if there is a vendor op implementation provided and calls it if so.
        /// Otherwise redirects back to Handle unsupported
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        protected virtual IWmsDriverResponse HandleVendorOp(string op)
        {
            //TODO - make sure vendor op is actually supported by the driver prior to trying to call it

            //verify if the op is supported!
            var version = GetParam("version");

            if(!SupportedVendorOperations.ContainsKey(version) || SupportedVendorOperations[version] == null ||  !SupportedVendorOperations[version].Exists(vop => string.Compare(vop, op, GetIgnoreCase()) == 0))
                return HandleUnsupported(op, false);

            var t = this.GetType();
            var m = t.GetMethod(
                "Handle" + op,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.IgnoreCase
            );

            if (m != null && m.ReturnType == typeof(IWmsDriverResponse))
            {
                //Note:
                //When a method invoked through MethodInfo.Invoke throws an exception, such exception
                //gets wrapped into an TargetInvocationException and becomes its inner exception. This seems to be a VisualStudio thing
                //when Tools -> Options -> Debugging -> General -> Enable Just My Code is on - the reason for this is that the exception leaves the 
                //boundaries of 'my' code and that's why is not handled by the IDE as one would expect.
                //
                //The solution therefore is to wrap the method invocation into a try catch, catch the TargetInvocationException:
                //try
                //{
                //    return (IWmsDriverResponse)m.Invoke(this, null);
                //}
                //catch (TargetInvocationException ex)
                //{
                //    if (ex.InnerException is WmsDriverException)
                //        throw ex.InnerException;
                //    else
                //        throw ex;
                //}
                //
                //or to create a delegate out of method info and invoke it instead:
                //var action = (Func<IWmsDriverResponse>)Delegate.CreateDelegate(typeof(Func<IWmsDriverResponse>), this, m);
                //return action();

                //more info:
                //http://stackoverflow.com/questions/2658908/why-is-targetinvocationexception-treated-as-uncaught-by-the-ide
                //http://stackoverflow.com/questions/4117228/reflection-methodinfo-invoke-catch-exceptions-from-inside-the-method

                var action = (Func<IWmsDriverResponse>)Delegate.CreateDelegate(typeof (Func<IWmsDriverResponse>), this, m);
                return action();
            }
            else
            {
                throw new WmsDriverException(string.Format("IMPLEMENTATION ERROR: Operation '{0}' is marked as supported but it is not implemented.", op));
            }
        }
    }
}
