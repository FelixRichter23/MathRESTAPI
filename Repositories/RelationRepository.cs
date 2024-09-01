using SqlKata.Execution;

namespace MathAPI.Repositories
{
    public class RelationRepository : IRelationRepository
    {
        private readonly QueryFactory _db;
        private readonly string _tableName = "Relations";
        public RelationRepository(QueryFactory db)
        {
            _db = db;
        }

        public void SaveRelation(int originCalculationID, int destinationCalculationID)
        {
            _db.Query(_tableName).Insert(new
            {
                OriginCalculationId = originCalculationID,
                DestinationCalculationId = destinationCalculationID
            });
        }

        public List<int> GetDependents(int destinationCalculationID)
        {
            return _db.Query(_tableName).Where("DestinationCalculationId", destinationCalculationID).Select("OriginCalculationId").Get<int>().ToList();
        }

        public void DeleteRelation(int originCalculationID)
        {
            _db.Query(_tableName).Where("OriginCalculationId", originCalculationID).Delete();
        }
    }
}
