using System;

namespace ExtensibleOpeningManager
{
    public static class Variables
    {
        public static readonly int round_system_value = 7;

        public static readonly string empty = "";

        public static readonly string default3dViewName = "SYS_DEFAULT_EOM";

        public static readonly string parameter_offset_down = "SYS_OFFSET_DOWN";
        public static readonly string parameter_offset_up = "SYS_OFFSET_UP";
        public static readonly string parameter_offset_bounds = "Расширение границ";
        public static readonly string parameter_width = "Ширина";
        public static readonly string parameter_height = "Высота";
        public static readonly string parameter_thickness = "Толщина";

        public static readonly string family_ar_round = "199_AR_ORW";
        public static readonly string family_ar_square = "199_AR_OSW";
        public static readonly string family_kr_round = "199_STR_ORW";
        public static readonly string family_kr_square = "199_STR_OSW";
        public static readonly string family_mep_round = "501_MEP_TRW";
        public static readonly string family_mep_square = "501_MEP_TSW";

        public static readonly string separator_element = "~0SE~";
        public static readonly string separator_sub_element = "~1SE~";
        public static readonly string separator_sub_sub_element = "~2SE~";

        public static readonly Guid schema = new Guid("f3a0ab5b-47d6-436a-882f-df9d03dd51d2");
        public static readonly Guid application = new Guid("af748280-4226-4c2e-9a2d-196f44e0ffbf");

        public static readonly string type_subelement_local_element = "6281c57ea2024d97a963761f8389a449";
        public static readonly string type_subelement_linked_element = "5d700707628c4b13a108611ac24f42b7";
        public static readonly string type_subelement_linked_instance = "16916e5868a14746883831a1ab4510b2";

        public static readonly string msg_created = "<!@%элемент~создан%@!>";
        public static readonly string msg_approved = "<!@%утверждено%@!>";
        public static readonly string msg_rejected = "<!@%отклонено%@!>";
        public static readonly string msg_autoJoined = "<!@%автопривязка%@!>";
    }
}
