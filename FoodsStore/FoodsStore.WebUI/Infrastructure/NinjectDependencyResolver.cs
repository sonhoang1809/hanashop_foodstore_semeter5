using FoodsStore.Domain.Abstract;
using FoodsStore.Domain.Concrete;
using Ninject;
using System;
using System.Collections.Generic;

using System.Web.Mvc;

namespace FoodsStore.WebUI.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernel)
        {
            this.kernel = kernel;
            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }

        private void AddBindings()
        {
            kernel.Bind<IProductRepository>().To<EFProductRepository>();
            kernel.Bind<IProductStatusRepository>().To<EFProductStatusRepository>();
            kernel.Bind<ICategoryRepository>().To<EFCategoryRepository>();
            kernel.Bind<IKindRepository>().To<EFKindRepository>();
            kernel.Bind<IUserRepository>().To<EFUserRepository>();
            kernel.Bind<IRoleRepository>().To<EFRoleRepository>();

            kernel.Bind<IBillRepository>().To<EFBillRepository>();
            kernel.Bind<IProductsInBillRepository>().To<EFProductsInBillRepository>();
        }
    }
}