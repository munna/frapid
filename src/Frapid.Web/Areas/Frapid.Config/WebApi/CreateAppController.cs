// ReSharper disable All
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Frapid.ApplicationState.Cache;
using Frapid.ApplicationState.Models;
using Frapid.DataAccess;
using Frapid.DataAccess.Models;
using Frapid.Framework;
using Frapid.Framework.Extensions;
using Frapid.Config.Entities;
using Frapid.Config.DataAccess;
namespace Frapid.Config.Api
{
    /// <summary>
    /// Provides a direct HTTP access to execute the function CreateApp.
    /// </summary>
    [RoutePrefix("api/v1.0/config/procedures/create-app")]
    public class CreateAppController : FrapidApiController
    {
        /// <summary>
        /// Login id of application user accessing this API.
        /// </summary>
        public long _LoginId { get; set; }

        /// <summary>
        /// User id of application user accessing this API.
        /// </summary>
        public int _UserId { get; set; }

        /// <summary>
        /// Currently logged in office id of application user accessing this API.
        /// </summary>
        public int _OfficeId { get; set; }

        /// <summary>
        /// The name of the database where queries are being executed on.
        /// </summary>
        public string _Catalog { get; set; }

        /// <summary>
        ///     The CreateApp repository.
        /// </summary>
        private readonly ICreateAppRepository repository;

        public class Annotation
        {
            public string AppName { get; set; }
            public string Name { get; set; }
            public string VersionNumber { get; set; }
            public string Publisher { get; set; }
            public DateTime PublishedOn { get; set; }
            public string Icon { get; set; }
            public string LandingUrl { get; set; }
            public string[] Dependencies { get; set; }
        }


        public CreateAppController()
        {
            this._LoginId = AppUsers.GetCurrent().View.LoginId.To<long>();
            this._UserId = AppUsers.GetCurrent().View.UserId.To<int>();
            this._OfficeId = AppUsers.GetCurrent().View.OfficeId.To<int>();
            this._Catalog = AppUsers.GetCatalog();

            this.repository = new CreateAppProcedure
            {
                _Catalog = this._Catalog,
                _LoginId = this._LoginId,
                _UserId = this._UserId
            };
        }

        public CreateAppController(ICreateAppRepository repository, string catalog, LoginView view)
        {
            this._LoginId = view.LoginId.To<long>();
            this._UserId = view.UserId.To<int>();
            this._OfficeId = view.OfficeId.To<int>();
            this._Catalog = catalog;

            this.repository = repository;
        }

        /// <summary>
        ///     Creates meta information of "create app" annotation.
        /// </summary>
        /// <returns>Returns the "create app" annotation meta information to perform CRUD operation.</returns>
        [AcceptVerbs("GET", "HEAD")]
        [Route("annotation")]
        [Route("~/api/config/procedures/create-app/annotation")]
        [Authorize]
        public EntityView GetAnnotation()
        {
            if (this._LoginId == 0)
            {
                return new EntityView();
            }
            return new EntityView
            {
                Columns = new List<EntityColumn>()
                                {
                                        new EntityColumn { ColumnName = "_app_name",  PropertyName = "AppName",  DataType = "string",  DbDataType = "text",  IsNullable = false,  IsPrimaryKey = false,  IsSerial = false,  Value = "",  MaxLength = 0 },
                                        new EntityColumn { ColumnName = "_name",  PropertyName = "Name",  DataType = "string",  DbDataType = "text",  IsNullable = false,  IsPrimaryKey = false,  IsSerial = false,  Value = "",  MaxLength = 0 },
                                        new EntityColumn { ColumnName = "_version_number",  PropertyName = "VersionNumber",  DataType = "string",  DbDataType = "text",  IsNullable = false,  IsPrimaryKey = false,  IsSerial = false,  Value = "",  MaxLength = 0 },
                                        new EntityColumn { ColumnName = "_publisher",  PropertyName = "Publisher",  DataType = "string",  DbDataType = "text",  IsNullable = false,  IsPrimaryKey = false,  IsSerial = false,  Value = "",  MaxLength = 0 },
                                        new EntityColumn { ColumnName = "_published_on",  PropertyName = "PublishedOn",  DataType = "DateTime",  DbDataType = "date",  IsNullable = false,  IsPrimaryKey = false,  IsSerial = false,  Value = "",  MaxLength = 0 },
                                        new EntityColumn { ColumnName = "_icon",  PropertyName = "Icon",  DataType = "string",  DbDataType = "text",  IsNullable = false,  IsPrimaryKey = false,  IsSerial = false,  Value = "",  MaxLength = 0 },
                                        new EntityColumn { ColumnName = "_landing_url",  PropertyName = "LandingUrl",  DataType = "string",  DbDataType = "text",  IsNullable = false,  IsPrimaryKey = false,  IsSerial = false,  Value = "",  MaxLength = 0 },
                                        new EntityColumn { ColumnName = "_dependencies",  PropertyName = "Dependencies",  DataType = "string",  DbDataType = "text[]",  IsNullable = false,  IsPrimaryKey = false,  IsSerial = false,  Value = "",  MaxLength = 0 }
                                }
            };
        }


        /// <summary>
        ///     Creates meta information of "create app" entity.
        /// </summary>
        /// <returns>Returns the "create app" meta information to perform CRUD operation.</returns>
        [AcceptVerbs("GET", "HEAD")]
        [Route("meta")]
        [Route("~/api/config/procedures/create-app/meta")]
        [Authorize]
        public EntityView GetEntityView()
        {
            if (this._LoginId == 0)
            {
                return new EntityView();
            }
            return new EntityView
            {
                Columns = new List<EntityColumn>()
                {
                }
            };
        }


        [AcceptVerbs("POST")]
        [Route("execute")]
        [Route("~/api/config/procedures/create-app/execute")]
        [Authorize]
        public void Execute([FromBody] Annotation annotation)
        {
            try
            {
                this.repository.AppName = annotation.AppName;
                this.repository.Name = annotation.Name;
                this.repository.VersionNumber = annotation.VersionNumber;
                this.repository.Publisher = annotation.Publisher;
                this.repository.PublishedOn = annotation.PublishedOn;
                this.repository.Icon = annotation.Icon;
                this.repository.LandingUrl = annotation.LandingUrl;
                this.repository.Dependencies = annotation.Dependencies;


                this.repository.Execute();
            }
            catch (UnauthorizedException)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Forbidden));
            }
            catch (DataAccessException ex)
            {
                throw new HttpResponseException(new HttpResponseMessage
                {
                    Content = new StringContent(ex.Message),
                    StatusCode = HttpStatusCode.InternalServerError
                });
            }
#if !DEBUG
            catch
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            }
#endif
        }
    }
}