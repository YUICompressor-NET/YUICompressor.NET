using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Yahoo.Yui.Compressor.Mozilla
{
    public enum Token
    {
        ERROR          = -1, // well-known as the only code < EOF
        EOF            = 0,  // end of file token - (not EOF_CHAR)
        EOL            = 1,  // end of line

        // Interpreter reuses the following as bytecodes
        FIRST_BYTECODE_TOKEN    = 2,

        ENTERWITH      = 2,
        LEAVEWITH      = 3,
        RETURN         = 4,
        GOTO           = 5,
        IFEQ           = 6,
        IFNE           = 7,
        SETNAME        = 8,
        BITOR          = 9,
        BITXOR         = 10,
        BITAND         = 11,
        EQ             = 12,
        NE             = 13,
        LT             = 14,
        LE             = 15,
        GT             = 16,
        GE             = 17,
        LSH            = 18,
        RSH            = 19,
        URSH           = 20,
        ADD            = 21,
        SUB            = 22,
        MUL            = 23,
        DIV            = 24,
        MOD            = 25,
        NOT            = 26,
        BITNOT         = 27,
        POS            = 28,
        NEG            = 29,
        NEW            = 30,
        DELPROP        = 31,
        TYPEOF         = 32,
        GETPROP        = 33,
        SETPROP        = 34,
        GETELEM        = 35,
        SETELEM        = 36,
        CALL           = 37,
        NAME           = 38,
        NUMBER         = 39,
        STRING         = 40,
        NULL           = 41,
        THIS           = 42,
        FALSE          = 43,
        TRUE           = 44,
        SHEQ           = 45,   // shallow equality (===)
        SHNE           = 46,   // shallow inequality (!==)
        REGEXP         = 47,
        BINDNAME       = 48,
        THROW          = 49,
        RETHROW        = 50, // rethrow caught execetion: catch (e if ) use it
        IN             = 51,
        INSTANCEOF     = 52,
        LOCAL_LOAD     = 53,
        GETVAR         = 54,
        SETVAR         = 55,
        CATCH_SCOPE    = 56,
        ENUM_INIT_KEYS = 57,
        ENUM_INIT_VALUES = 58,
        ENUM_NEXT      = 59,
        ENUM_ID        = 60,
        THISFN         = 61,
        RETURN_RESULT  = 62, // to return prevoisly stored return result
        ARRAYLIT       = 63, // array literal
        OBJECTLIT      = 64, // object literal
        GET_REF        = 65, // *reference
        SET_REF        = 66, // *reference    = something
        DEL_REF        = 67, // delete reference
        REF_CALL       = 68, // f(args)    = something or f(args)++
        REF_SPECIAL    = 69, // reference for special properties like __proto

        // For XML support:
        DEFAULTNAMESPACE = 70, // default xml namespace =
        ESCXMLATTR     = 71,
        ESCXMLTEXT     = 72,
        REF_MEMBER     = 73, // Reference for x.@y, x..y etc.
        REF_NS_MEMBER  = 74, // Reference for x.ns::y, x..ns::y etc.
        REF_NAME       = 75, // Reference for @y, @[y] etc.
        REF_NS_NAME    = 76, // Reference for ns::y, @ns::y@[y] etc.

        LAST_BYTECODE_TOKEN    = REF_NS_NAME,

        TRY            = 77,
        SEMI           = 78,  // semicolon
        LB             = 79,  // left and right brackets
        RB             = 80,
        LC             = 81,  // left and right curlies (braces)
        RC             = 82,
        LP             = 83,  // left and right parentheses
        RP             = 84,
        COMMA          = 85,  // comma operator

        ASSIGN         = 86,  // simple assignment  (=)
        ASSIGN_BITOR   = 87,  // |=
        ASSIGN_BITXOR  = 88,  // ^=
        ASSIGN_BITAND  = 89,  // |=
        ASSIGN_LSH     = 90,  // <<=
        ASSIGN_RSH     = 91,  // >>=
        ASSIGN_URSH    = 92,  // >>>=
        ASSIGN_ADD     = 93,  // +=
        ASSIGN_SUB     = 94,  // -=
        ASSIGN_MUL     = 95,  // *=
        ASSIGN_DIV     = 96,  // /=
        ASSIGN_MOD     = 97,  // %=

        FIRST_ASSIGN   = ASSIGN,
        LAST_ASSIGN    = ASSIGN_MOD,

        HOOK           = 98, // conditional (?:)
        COLON          = 99,
        OR             = 100, // logical or (||)
        AND            = 101, // logical and (&&)
        INC            = 102, // increment/decrement (++ --)
        DEC            = 103,
        DOT            = 104, // member operator (.)
        FUNCTION       = 105, // function keyword
        EXPORT         = 106, // export keyword
        IMPORT         = 107, // import keyword
        IF             = 108, // if keyword
        ELSE           = 109, // else keyword
        SWITCH         = 110, // switch keyword
        CASE           = 111, // case keyword
        DEFAULT        = 112, // default keyword
        WHILE          = 113, // while keyword
        DO             = 114, // do keyword
        FOR            = 115, // for keyword
        BREAK          = 116, // break keyword
        CONTINUE       = 117, // continue keyword
        VAR            = 118, // var keyword
        WITH           = 119, // with keyword
        CATCH          = 120, // catch keyword
        FINALLY        = 121, // finally keyword
        VOID           = 122, // void keyword
        RESERVED       = 123, // reserved keywords

        EMPTY          = 124,

        // Types used for the parse tree - these never get returned by the scanner.
        BLOCK          = 125, // statement block
        LABEL          = 126, // label
        TARGET         = 127,
        LOOP           = 128,
        EXPR_VOID      = 129, // expression statement in functions
        EXPR_RESULT    = 130, // expression statement in scripts
        JSR            = 131,
        SCRIPT         = 132, // top-level node for entire script
        TYPEOFNAME     = 133, // for typeof(simple-name)
        USE_STACK      = 134,
        SETPROP_OP     = 135, // x.y op= something
        SETELEM_OP     = 136, // x[y] op= something
        LOCAL_BLOCK    = 137,
        SET_REF_OP     = 138, // *reference op= something

        // For XML support:
        DOTDOT         = 139,  // member operator (..)
        COLONCOLON     = 140,  // namespace::name
        XML            = 141,  // XML type
        DOTQUERY       = 142,  // .() -- e.g., x.emps.emp.(name == "terry")
        XMLATTR        = 143,  // @
        XMLEND         = 144,

        // Optimizer-only-tokens
        TO_OBJECT      = 145,
        TO_DOUBLE      = 146,

        GET            = 147,  // JS 1.5 get pseudo keyword
        SET            = 148,  // JS 1.5 set pseudo keyword
        CONST          = 149,
        SETCONST       = 150,
        SETCONSTVAR    = 151,

        SPECIALCOMMENT = 152, // Internet Explorer conditional comment

        LAST_TOKEN     = 153
    }

    public static partial class Extensions
    {
        public static string Description(this Token value)
        {
            switch (value)
            {
                case Token.ERROR:           return "ERROR";
                case Token.EOF:             return "EOF";
                case Token.EOL:             return "EOL";
                case Token.ENTERWITH:       return "ENTERWITH";
                case Token.LEAVEWITH:       return "LEAVEWITH";
                case Token.RETURN:          return "RETURN";
                case Token.GOTO:            return "GOTO";
                case Token.IFEQ:            return "IFEQ";
                case Token.IFNE:            return "IFNE";
                case Token.SETNAME:         return "SETNAME";
                case Token.BITOR:           return "BITOR";
                case Token.BITXOR:          return "BITXOR";
                case Token.BITAND:          return "BITAND";
                case Token.EQ:              return "EQ";
                case Token.NE:              return "NE";
                case Token.LT:              return "LT";
                case Token.LE:              return "LE";
                case Token.GT:              return "GT";
                case Token.GE:              return "GE";
                case Token.LSH:             return "LSH";
                case Token.RSH:             return "RSH";
                case Token.URSH:            return "URSH";
                case Token.ADD:             return "ADD";
                case Token.SUB:             return "SUB";
                case Token.MUL:             return "MUL";
                case Token.DIV:             return "DIV";
                case Token.MOD:             return "MOD";
                case Token.NOT:             return "NOT";
                case Token.BITNOT:          return "BITNOT";
                case Token.POS:             return "POS";
                case Token.NEG:             return "NEG";
                case Token.NEW:             return "NEW";
                case Token.DELPROP:         return "DELPROP";
                case Token.TYPEOF:          return "TYPEOF";
                case Token.GETPROP:         return "GETPROP";
                case Token.SETPROP:         return "SETPROP";
                case Token.GETELEM:         return "GETELEM";
                case Token.SETELEM:         return "SETELEM";
                case Token.CALL:            return "CALL";
                case Token.NAME:            return "NAME";
                case Token.NUMBER:          return "NUMBER";
                case Token.STRING:          return "STRING";
                case Token.NULL:            return "NULL";
                case Token.THIS:            return "THIS";
                case Token.FALSE:           return "FALSE";
                case Token.TRUE:            return "TRUE";
                case Token.SHEQ:            return "SHEQ";
                case Token.SHNE:            return "SHNE";
                case Token.REGEXP:          return "OBJECT";
                case Token.BINDNAME:        return "BINDNAME";
                case Token.THROW:           return "THROW";
                case Token.RETHROW:         return "RETHROW";
                case Token.IN:              return "IN";
                case Token.INSTANCEOF:      return "INSTANCEOF";
                case Token.LOCAL_LOAD:      return "LOCAL_LOAD";
                case Token.GETVAR:          return "GETVAR";
                case Token.SETVAR:          return "SETVAR";
                case Token.CATCH_SCOPE:     return "CATCH_SCOPE";
                case Token.ENUM_INIT_KEYS:  return "ENUM_INIT_KEYS";
                case Token.ENUM_INIT_VALUES:  return "ENUM_INIT_VALUES";
                case Token.ENUM_NEXT:       return "ENUM_NEXT";
                case Token.ENUM_ID:         return "ENUM_ID";
                case Token.THISFN:          return "THISFN";
                case Token.RETURN_RESULT:   return "RETURN_RESULT";
                case Token.ARRAYLIT:        return "ARRAYLIT";
                case Token.OBJECTLIT:       return "OBJECTLIT";
                case Token.GET_REF:         return "GET_REF";
                case Token.SET_REF:         return "SET_REF";
                case Token.DEL_REF:         return "DEL_REF";
                case Token.REF_CALL:        return "REF_CALL";
                case Token.REF_SPECIAL:     return "REF_SPECIAL";
                case Token.DEFAULTNAMESPACE:return "DEFAULTNAMESPACE";
                case Token.ESCXMLTEXT:      return "ESCXMLTEXT";
                case Token.ESCXMLATTR:      return "ESCXMLATTR";
                case Token.REF_MEMBER:      return "REF_MEMBER";
                case Token.REF_NS_MEMBER:   return "REF_NS_MEMBER";
                case Token.REF_NAME:        return "REF_NAME";
                case Token.REF_NS_NAME:     return "REF_NS_NAME";
                case Token.TRY:             return "TRY";
                case Token.SEMI:            return "SEMI";
                case Token.LB:              return "LB";
                case Token.RB:              return "RB";
                case Token.LC:              return "LC";
                case Token.RC:              return "RC";
                case Token.LP:              return "LP";
                case Token.RP:              return "RP";
                case Token.COMMA:           return "COMMA";
                case Token.ASSIGN:          return "ASSIGN";
                case Token.ASSIGN_BITOR:    return "ASSIGN_BITOR";
                case Token.ASSIGN_BITXOR:   return "ASSIGN_BITXOR";
                case Token.ASSIGN_BITAND:   return "ASSIGN_BITAND";
                case Token.ASSIGN_LSH:      return "ASSIGN_LSH";
                case Token.ASSIGN_RSH:      return "ASSIGN_RSH";
                case Token.ASSIGN_URSH:     return "ASSIGN_URSH";
                case Token.ASSIGN_ADD:      return "ASSIGN_ADD";
                case Token.ASSIGN_SUB:      return "ASSIGN_SUB";
                case Token.ASSIGN_MUL:      return "ASSIGN_MUL";
                case Token.ASSIGN_DIV:      return "ASSIGN_DIV";
                case Token.ASSIGN_MOD:      return "ASSIGN_MOD";
                case Token.HOOK:            return "HOOK";
                case Token.COLON:           return "COLON";
                case Token.OR:              return "OR";
                case Token.AND:             return "AND";
                case Token.INC:             return "INC";
                case Token.DEC:             return "DEC";
                case Token.DOT:             return "DOT";
                case Token.FUNCTION:        return "FUNCTION";
                case Token.EXPORT:          return "EXPORT";
                case Token.IMPORT:          return "IMPORT";
                case Token.IF:              return "IF";
                case Token.ELSE:            return "ELSE";
                case Token.SWITCH:          return "SWITCH";
                case Token.CASE:            return "CASE";
                case Token.DEFAULT:         return "DEFAULT";
                case Token.WHILE:           return "WHILE";
                case Token.DO:              return "DO";
                case Token.FOR:             return "FOR";
                case Token.BREAK:           return "BREAK";
                case Token.CONTINUE:        return "CONTINUE";
                case Token.VAR:             return "VAR";
                case Token.WITH:            return "WITH";
                case Token.CATCH:           return "CATCH";
                case Token.FINALLY:         return "FINALLY";
                case Token.RESERVED:        return "RESERVED";
                case Token.EMPTY:           return "EMPTY";
                case Token.BLOCK:           return "BLOCK";
                case Token.LABEL:           return "LABEL";
                case Token.TARGET:          return "TARGET";
                case Token.LOOP:            return "LOOP";
                case Token.EXPR_VOID:       return "EXPR_VOID";
                case Token.EXPR_RESULT:     return "EXPR_RESULT";
                case Token.JSR:             return "JSR";
                case Token.SCRIPT:          return "SCRIPT";
                case Token.TYPEOFNAME:      return "TYPEOFNAME";
                case Token.USE_STACK:       return "USE_STACK";
                case Token.SETPROP_OP:      return "SETPROP_OP";
                case Token.SETELEM_OP:      return "SETELEM_OP";
                case Token.LOCAL_BLOCK:     return "LOCAL_BLOCK";
                case Token.SET_REF_OP:      return "SET_REF_OP";
                case Token.DOTDOT:          return "DOTDOT";
                case Token.COLONCOLON:      return "COLONCOLON";
                case Token.XML:             return "XML";
                case Token.DOTQUERY:        return "DOTQUERY";
                case Token.XMLATTR:         return "XMLATTR";
                case Token.XMLEND:          return "XMLEND";
                case Token.TO_OBJECT:       return "TO_OBJECT";
                case Token.TO_DOUBLE:       return "TO_DOUBLE";
                case Token.GET:             return "GET";
                case Token.SET:             return "SET";
                case Token.CONST:           return "CONST";
                case Token.SETCONST:        return "SETCONST";
            }

            return null;
        }

        public static int ToInt(this Token value)
        {
            return Convert.ToInt32(value);
        }

        public static char ToChar(this Token value)
        {
            return Convert.ToChar(value.ToInt());
        }
    }
}