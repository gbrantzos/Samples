using System;

namespace Workbench
{
    public abstract class EntityID
    {
        public EntityID(long value)
        {
            Value = value == 0 ? -1 : value;
        }
        public long Value { get; set; }
    }

    public abstract class Entity<TSnapshot> where TSnapshot : Snapshot
    {
        public abstract TSnapshot GetSnapshot();
    }

    public abstract class Snapshot
    {
        public long ID { get; set; }
    }


    public class CompanyID : EntityID
    {
        public CompanyID(long value) : base(value) { }
        public CompanyID() : this(0) { }
    }

    public sealed class Company : Entity<CompanySnapshot>
    {
        // Domain objects should have a private parameterless constructor
        private Company() { }

        // Properties
        public CompanyID CompanyID { get; private set; }
        public string Name         { get; set; }
        public string ErpCode      { get; set; }
        public string Comments     { get; set; }

        // By convention we use a factory like method for new instances or from snapshot
        public static Company Create(string name, string erpCode, string comments) =>
            FromSnapshot(new CompanySnapshot
                {
                    Name     = name,
                    ErpCode  = erpCode,
                    Comments = comments
                });

        public static Company FromSnapshot(CompanySnapshot snapshot)
        {
            // Business rules - Validations
            if (snapshot.Name == null)
                throw new ArgumentNullException(nameof(snapshot.Name));

            return new Company
            {
                CompanyID = new CompanyID(snapshot.ID),
                Name      = snapshot.Name,
                ErpCode   = snapshot.ErpCode,
                Comments  = snapshot.Comments
            };
        }

        public override CompanySnapshot GetSnapshot() =>
            new CompanySnapshot
            {
                ID       = CompanyID.Value,
                Name     = Name,
                ErpCode  = ErpCode,
                Comments = Comments
            };
    }

    public class CompanySnapshot : Snapshot
    {
        public string Name { get; set; }
        public string ErpCode { get; set; }
        public string Comments { get; set; }
    }
}
