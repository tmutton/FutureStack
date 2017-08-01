using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using SimpleInjector;
using SimpleInjector.Advanced;

//See: https://simpleinjector.readthedocs.io/en/latest/extensibility.html#overriding-constructor-resolution-behavior
//Frankly single constructor only is just nonsense. But folks want to use SimpleInjector, so we just override its
//behaviour everywhere

namespace ToDoCore.Adaptors
{
    public class MostResolvableConstructorBehavior : IConstructorResolutionBehavior {
        private readonly Container container;

        public MostResolvableConstructorBehavior(Container container) {
            this.container = container;
        }

        private bool IsCalledDuringRegistrationPhase {
            [DebuggerStepThrough]
            get { return !this.container.IsLocked(); }
        }

        [DebuggerStepThrough]
        public ConstructorInfo GetConstructor(Type service, Type implementation) {
            var constructors = implementation.GetConstructors();
            return (
                    from ctor in constructors
                    let parameters = ctor.GetParameters()
                    where this.IsCalledDuringRegistrationPhase
                          || constructors.Length == 1
                          || parameters.All(p => this.CanBeResolved(p, service, implementation))
                    orderby parameters.Length descending
                    select ctor)
                .First();
        }

        [DebuggerStepThrough]
        private bool CanBeResolved(ParameterInfo p, Type service, Type implementation) {
            return this.container.GetRegistration(p.ParameterType) != null ||
                   this.CanBuildType(p, service, implementation);
        }

        [DebuggerStepThrough]
        private bool CanBuildType(ParameterInfo p, Type service, Type implementation) {
            try {
                this.container.Options.DependencyInjectionBehavior.BuildExpression(
                    new InjectionConsumerInfo(service, implementation, p));
                return true;
            } catch (ActivationException) {
                return false;
            }
        }
    }
}