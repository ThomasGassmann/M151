namespace TaobaoExpress.Controllers
{
    using System;
    using System.Data.Entity.Core;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Web.Mvc;
    using TaobaoExpress.DataAccess;
    using TaobaoExpress.Services.BusinessRules;
    using TaobaoExpress.Services.UoW;
    using Unity.Interception.Utilities;

    public class TaobaoExpressBaseController : Controller
    {
        private readonly IUnitOfWorkFactory unitOfWorkFactory;

        public TaobaoExpressBaseController(IUnitOfWorkFactory unitOfWorkFactory)
        {
            this.unitOfWorkFactory = unitOfWorkFactory;
        }

        protected ActionResult Save<T>(T entity, Func<IUnitOfWork, T, ActionResult> successAction, string invalidModelStateView = null) where T : class, IIdentifiable
        {
            if (!this.ModelState.IsValid)
            {
                return string.IsNullOrEmpty(invalidModelStateView) ? this.View() : this.View(invalidModelStateView);
            }
            
            return ExecuteInUnitOfWork(unitOfWork =>
            {
                var repository = unitOfWork.CreateRepository<T>();
                var existing = repository.Get(entity.Id);
                (existing == null ? repository.Add : (Action<T>)repository.Update)(entity);
            }, unitOfWork => successAction(unitOfWork, entity));
        }

        protected ActionResult ExecuteInUnitOfWork(
            Action<IUnitOfWork> action,
            Func<IUnitOfWork, ActionResult> successAction,
            Func<ActionResult> badRequestAction = null,
            Func<IUnitOfWork, ActionResult> badRequestWithUnitOfWork = null)
        {
            ActionResult badRequest(IUnitOfWork unitOfWork)
            {
                if (badRequestWithUnitOfWork != null)
                {
                    return badRequestWithUnitOfWork(unitOfWork);
                }

                return (badRequestAction ?? this.View)();
            }

            using (var unitOfWork = this.unitOfWorkFactory.CreateUnitOfWork())
            {
                try
                {
                    action(unitOfWork);
                    unitOfWork.Save();
                    return successAction(unitOfWork);
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("OPTIMISTIC_CONCURRENCY", "The given entity was updated after you loaded it. Please try to refresh your page before continuing.");
                    return badRequest(unitOfWork);
                }
                catch (BusinessRuleException ex)
                {
                    ModelState.AddModelError("BUSINESS_RULE", ex.Message);
                    return badRequest(unitOfWork);
                }
                catch (DbEntityValidationException ex)
                {
                    var messages = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors);
                    foreach (var message in messages)
                    {
                        ModelState.AddModelError(message.PropertyName, message.ErrorMessage);
                    }

                    return badRequest(unitOfWork);
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("ERROR", ex);
                    return badRequest(unitOfWork);
                }
                catch (UpdateException ex)
                {
                    ModelState.AddModelError("ERROR", ex);
                    return badRequest(unitOfWork);
                }
            }
        }
    }
}