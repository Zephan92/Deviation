using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Deviation.Data.LootPool;
using Deviation.Server.Controllers;

namespace Deviation.Server.IoC
{
	public class WindsorInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(AllTypes.FromAssemblyNamed("Deviation.Data")
								.Where(type => type.IsPublic)
								.WithService.FirstInterface().LifestyleSingleton());
			container.Register(Classes.FromThisAssembly().BasedOn<LootPoolAPIController>().LifestyleTransient());
			container.Register(Classes.FromThisAssembly().BasedOn<PlayerAccountAPIController>().LifestyleTransient());

		}
	}
}