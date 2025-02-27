using Mangrove.Data;

namespace Mangrove.Models {
    public class InfoStagesOfIndividualModel {
        public string NameMangrove { get; set; } = null!;
        public TblIndividual Individual { get; set; } = null!;

        public List<Stage> Stages { get; set; } = null!;
    }

    public class Stage {
        public TblStage info { get; set; } = null!;
        public List<TblPhoto> photo { get; set; } = null!;

    }
}
