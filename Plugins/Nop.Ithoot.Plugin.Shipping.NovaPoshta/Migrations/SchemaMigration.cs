using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Ithoot.Plugin.Shipping.NovaPoshta.Domain;

namespace Nop.Ithoot.Plugin.Shipping.NovaPoshta.Data
{
    [NopMigration("2023/04/04 08:40:55:1687541", "Shipping.NovaPoshta base schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            //Create.TableFor<NovaPoshtaCity>();
            //Create.TableFor<NovaPoshtaWarehouse>();
        }
    }
}