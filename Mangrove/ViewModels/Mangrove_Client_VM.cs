namespace Mangrove.ViewModels {
	public class Mangrove_Client_VM {
		public string Id { get; set; } = null!;
		public string MainImage { get; set; } = null!;
		public string Name { get; set; } = null!;
		public string CommonName { get; set; } = null!;
		public string ScientificName { get; set; } = null!;
		public string Familia { get; set; } = null!;
		public string Use { get; set; } = null!;
		public string Morphology { get; set; } = null!;
		public string Ecology { get; set; } = null!;
		public string Distribution { get; set; } = null!;
		public string ConservationStatus { get; set; } = null!;
		public long View { get; set; }

		public List<Photo_Mangrove_Client_VM> Photos = new List<Photo_Mangrove_Client_VM>();
		public List<Individual_Mangrove_Client_VM> Individuals = new List<Individual_Mangrove_Client_VM>();
	}

	public class Photo_Mangrove_Client_VM {
		public string Image { get; set; } = null!;
		public string Note { get; set; } = null!;
	}

	public class Individual_Mangrove_Client_VM {
		public string Id { get; set; } = null!;
		public string UpdateLast { get; set; } = null!;
		public long View { get; set; }
		public int NumberStages { get; set; }
		public string Position { get; set; } = null!;
		public string QrName { get; set; } = null!;
	}
}
