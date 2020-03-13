using FluentMigrator;

namespace ContactManager.Migrations
{
    [Migration(20200312001, description: "Create Events table")]
    public class EventsTable : Migration
    {
        public override void Up()
        {
            Create.Table("Events")
                .WithColumn("EventID").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("AggregateID").AsInt64().NotNullable()
                .WithColumn("Version").AsInt32().NotNullable()
                .WithColumn("CreatedAt").AsDateTime2().NotNullable()
                .WithColumn("EventType").AsString()
                .WithColumn("Payload").AsString();
        }

        public override void Down() { }
    }
}
