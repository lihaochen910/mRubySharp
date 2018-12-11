
namespace CandyFramework.mRuby
{
    /// <summary>
    /// mruby中的值类型
    /// (详细参见include\mruby\value.h)
    /// mruby 2.0.0 2018-4-27
    /// </summary>
    public enum mrb_vtype
    {
        MRB_TT_FALSE = 0,   /*   0 */
        MRB_TT_FREE,        /*   1 */
        MRB_TT_TRUE,        /*   2 */
        MRB_TT_FIXNUM,      /*   3 */
        MRB_TT_SYMBOL,      /*   4 */
        MRB_TT_UNDEF,       /*   5 */
        MRB_TT_FLOAT,       /*   6 */
        MRB_TT_CPTR,        /*   7 */
        MRB_TT_OBJECT,      /*   8 */
        MRB_TT_CLASS,       /*   9 */
        MRB_TT_MODULE,      /*  10 */
        MRB_TT_ICLASS,      /*  11 */
        MRB_TT_SCLASS,      /*  12 */
        MRB_TT_PROC,        /*  13 */
        MRB_TT_ARRAY,       /*  14 */
        MRB_TT_HASH,        /*  15 */
        MRB_TT_STRING,      /*  16 */
        MRB_TT_RANGE,       /*  17 */
        MRB_TT_EXCEPTION,   /*  18 */
        MRB_TT_FILE,        /*  19 */
        MRB_TT_ENV,         /*  20 */
        MRB_TT_DATA,        /*  21 */
        MRB_TT_FIBER,       /*  22 */
        MRB_TT_ISTRUCT,     /*  23 */
        MRB_TT_BREAK,       /*  24 */
        MRB_TT_MAXDEFINE    /*  25 */
    }
}
