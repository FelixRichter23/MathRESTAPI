namespace MathAPI.Repositories
{
    public interface IRelationRepository
    {
        public void SaveRelation(int originCalculationID, int destinationCalculationID);

        public List<int> GetDependents(int destinationCalculationID);

        public void DeleteRelation(int originCalculationID);
    }
}
