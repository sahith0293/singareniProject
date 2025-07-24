using System.Collections.Generic;

namespace VSTAPP.Models
{
    public class DesignationSOPModel
    {
        public Dictionary<string, List<string>> Designations { get; set; }
        public List<SOPItem> SOPs { get; set; }

        public DesignationSOPModel()
        {
            Designations = new Dictionary<string, List<string>>();
            SOPs = new List<SOPItem>();
        }
    }

    public class SOPItem
    {
        public string name { get; set; }
        public string path { get; set; }
    }

    public class TrainingSessionData
    {
        public string Area { get; set; }
        public string Mine { get; set; }
        public string Designation { get; set; }
        public string SOPName { get; set; }
        public string SOPPath { get; set; }
        public System.DateTime SessionStartTime { get; set; }
        public string SessionId { get; set; }

        public TrainingSessionData()
        {
            SessionId = System.Guid.NewGuid().ToString();
        }
    }
}