using System.Collections.Generic;

namespace ExtensibleOpeningManager.Common
{
    public static class Collections
    {
        public enum Department { AR, KR, MEP }
        public enum SymbolType { Round, Square }
        public enum Status { Applied, Rejected, Null }
        public enum VisibleStatus { Ok, Warning, Alert }
        public enum SubStatus { Applied, Rejected, Changed, NotFound }
        public enum WallStatus { Ok, NotCommited, NotFound }
        public enum ExtensibleParameter { Document, Instance, Status, Wall, SubElementsCollection, CommentsCollection }
        public static readonly List<string> ExtensibleParameter_String = new List<string> { "Document", "Instance", "Status", "Wall", "SubElementsCollection", "CommentsCollection" };
        public enum PickTypeOptions { Instance, Element }
        public enum PickOptions { Local, References }
        public enum Icon { OpenManager, Settings }
        public enum ImageButton { Approve, Reject, Swap, Reset, Update, Group, Ungroup, Apply, ApplyWall, SetWall, ApplySubElements, AddSubElements, SetOffset }
        public enum ImageMonitor { Error, Ok, Remove, Element_Approved, Element_Errored, Element_Unapproved, Update, Waiting, Warning, Request }
        public enum SubElementStatus { Ok, NotCommited, NotFound, NotApproved, NotCommitedInside }
        public enum RemarkType { Request, Answer_Ok, Answer_No }
    }
}
