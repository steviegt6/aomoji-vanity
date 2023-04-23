using System.Diagnostics.CodeAnalysis;

namespace AomojiVanity.API.ModCall;

public class ModCallTypeValidator {
    public static bool Validate<T1>(object[] args, [NotNullWhen(true)] out T1? t1) {
        t1 = default;

        if (args.Length != 1)
            return false;

        return args[0] is T1;
    }

    public static bool Validate<T1, T2>(object[] args, [NotNullWhen(true)] out T1? t1, [NotNullWhen(true)] out T2? t2) {
        t1 = default;
        t2 = default;

        if (args.Length != 2)
            return false;

        return args[0] is T1 && args[1] is T2;
    }

    public static bool Validate<T1, T2, T3>(object[] args, [NotNullWhen(true)] out T1? t1, [NotNullWhen(true)] out T2? t2, [NotNullWhen(true)] out T3? t3) {
        t1 = default;
        t2 = default;
        t3 = default;

        if (args.Length != 3)
            return false;

        return args[0] is T1 && args[1] is T2 && args[2] is T3;
    }

    public static bool Validate<T1, T2, T3, T4>(object?[] args, [NotNullWhen(true)] out T1? t1, [NotNullWhen(true)] out T2? t2, [NotNullWhen(true)] out T3? t3, [NotNullWhen(true)] out T4? t4) {
        t1 = default;
        t2 = default;
        t3 = default;
        t4 = default;

        if (args.Length != 4)
            return false;

        return args[0] is T1 && args[1] is T2 && args[2] is T3 && args[3] is T4;
    }

    public static bool Validate<T1, T2, T3, T4, T5>(object[] args, [NotNullWhen(true)] out T1? t1, [NotNullWhen(true)] out T2? t2, [NotNullWhen(true)] out T3? t3, [NotNullWhen(true)] out T4? t4, [NotNullWhen(true)] out T5? t5) {
        t1 = default;
        t2 = default;
        t3 = default;
        t4 = default;
        t5 = default;

        if (args.Length != 5)
            return false;

        return args[0] is T1 && args[1] is T2 && args[2] is T3 && args[3] is T4 && args[4] is T5;
    }

    public static bool Validate<T1, T2, T3, T4, T5, T6>(
        object[] args,
        [NotNullWhen(true)]
        out T1? t1,
        [NotNullWhen(true)]
        out T2? t2,
        [NotNullWhen(true)]
        out T3? t3,
        [NotNullWhen(true)]
        out T4? t4,
        [NotNullWhen(true)]
        out T5? t5,
        [NotNullWhen(true)]
        out T6? t6
    ) {
        t1 = default;
        t2 = default;
        t3 = default;
        t4 = default;
        t5 = default;
        t6 = default;

        if (args.Length != 6)
            return false;

        return args[0] is T1 && args[1] is T2 && args[2] is T3 && args[3] is T4 && args[4] is T5 && args[5] is T6;
    }

    public static bool Validate<T1, T2, T3, T4, T5, T6, T7>(
        object[] args,
        [NotNullWhen(true)]
        out T1? t1,
        [NotNullWhen(true)]
        out T2? t2,
        [NotNullWhen(true)]
        out T3? t3,
        [NotNullWhen(true)]
        out T4? t4,
        [NotNullWhen(true)]
        out T5? t5,
        [NotNullWhen(true)]
        out T6? t6,
        [NotNullWhen(true)]
        out T7? t7
    ) {
        t1 = default;
        t2 = default;
        t3 = default;
        t4 = default;
        t5 = default;
        t6 = default;
        t7 = default;

        if (args.Length != 7)
            return false;

        return args[0] is T1 && args[1] is T2 && args[2] is T3 && args[3] is T4 && args[4] is T5 && args[5] is T6 && args[6] is T7;
    }

    public static bool Validate<T1, T2, T3, T4, T5, T6, T7, T8>(
        object[] args,
        [NotNullWhen(true)]
        out T1? t1,
        [NotNullWhen(true)]
        out T2? t2,
        [NotNullWhen(true)]
        out T3? t3,
        [NotNullWhen(true)]
        out T4? t4,
        [NotNullWhen(true)]
        out T5? t5,
        [NotNullWhen(true)]
        out T6? t6,
        [NotNullWhen(true)]
        out T7? t7,
        [NotNullWhen(true)]
        out T8? t8
    ) {
        t1 = default;
        t2 = default;
        t3 = default;
        t4 = default;
        t5 = default;
        t6 = default;
        t7 = default;
        t8 = default;

        if (args.Length != 8)
            return false;

        return args[0] is T1 && args[1] is T2 && args[2] is T3 && args[3] is T4 && args[4] is T5 && args[5] is T6 && args[6] is T7 && args[7] is T8;
    }

    public static bool Validate<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
        object[] args,
        [NotNullWhen(true)]
        out T1? t1,
        [NotNullWhen(true)]
        out T2? t2,
        [NotNullWhen(true)]
        out T3? t3,
        [NotNullWhen(true)]
        out T4? t4,
        [NotNullWhen(true)]
        out T5? t5,
        [NotNullWhen(true)]
        out T6? t6,
        [NotNullWhen(true)]
        out T7? t7,
        [NotNullWhen(true)]
        out T8? t8,
        [NotNullWhen(true)]
        out T9? t9
    ) {
        t1 = default;
        t2 = default;
        t3 = default;
        t4 = default;
        t5 = default;
        t6 = default;
        t7 = default;
        t8 = default;
        t9 = default;

        if (args.Length != 9)
            return false;

        return args[0] is T1 && args[1] is T2 && args[2] is T3 && args[3] is T4 && args[4] is T5 && args[5] is T6 && args[6] is T7 && args[7] is T8 && args[8] is T9;
    }

    public static bool Validate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
        object[] args,
        [NotNullWhen(true)]
        out T1? t1,
        [NotNullWhen(true)]
        out T2? t2,
        [NotNullWhen(true)]
        out T3? t3,
        [NotNullWhen(true)]
        out T4? t4,
        [NotNullWhen(true)]
        out T5? t5,
        [NotNullWhen(true)]
        out T6? t6,
        [NotNullWhen(true)]
        out T7? t7,
        [NotNullWhen(true)]
        out T8? t8,
        [NotNullWhen(true)]
        out T9? t9,
        [NotNullWhen(true)]
        out T10? t10
    ) {
        t1 = default;
        t2 = default;
        t3 = default;
        t4 = default;
        t5 = default;
        t6 = default;
        t7 = default;
        t8 = default;
        t9 = default;
        t10 = default;

        if (args.Length != 10)
            return false;

        return args[0] is T1 && args[1] is T2 && args[2] is T3 && args[3] is T4 && args[4] is T5 && args[5] is T6 && args[6] is T7 && args[7] is T8 && args[8] is T9 && args[9] is T10;
    }

    public static bool Validate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
        object[] args,
        [NotNullWhen(true)]
        out T1? t1,
        [NotNullWhen(true)]
        out T2? t2,
        [NotNullWhen(true)]
        out T3? t3,
        [NotNullWhen(true)]
        out T4? t4,
        [NotNullWhen(true)]
        out T5? t5,
        [NotNullWhen(true)]
        out T6? t6,
        [NotNullWhen(true)]
        out T7? t7,
        [NotNullWhen(true)]
        out T8? t8,
        [NotNullWhen(true)]
        out T9? t9,
        [NotNullWhen(true)]
        out T10? t10,
        [NotNullWhen(true)]
        out T11? t11
    ) {
        t1 = default;
        t2 = default;
        t3 = default;
        t4 = default;
        t5 = default;
        t6 = default;
        t7 = default;
        t8 = default;
        t9 = default;
        t10 = default;
        t11 = default;

        if (args.Length != 11)
            return false;

        return args[0] is T1 && args[1] is T2 && args[2] is T3 && args[3] is T4 && args[4] is T5 && args[5] is T6 && args[6] is T7 && args[7] is T8 && args[8] is T9 && args[9] is T10 && args[10] is T11;
    }

    public static bool Validate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
        object[] args,
        [NotNullWhen(true)]
        out T1? t1,
        [NotNullWhen(true)]
        out T2? t2,
        [NotNullWhen(true)]
        out T3? t3,
        [NotNullWhen(true)]
        out T4? t4,
        [NotNullWhen(true)]
        out T5? t5,
        [NotNullWhen(true)]
        out T6? t6,
        [NotNullWhen(true)]
        out T7? t7,
        [NotNullWhen(true)]
        out T8? t8,
        [NotNullWhen(true)]
        out T9? t9,
        [NotNullWhen(true)]
        out T10? t10,
        [NotNullWhen(true)]
        out T11? t11,
        [NotNullWhen(true)]
        out T12? t12
    ) {
        t1 = default;
        t2 = default;
        t3 = default;
        t4 = default;
        t5 = default;
        t6 = default;
        t7 = default;
        t8 = default;
        t9 = default;
        t10 = default;
        t11 = default;
        t12 = default;

        if (args.Length != 12)
            return false;

        return args[0] is T1 && args[1] is T2 && args[2] is T3 && args[3] is T4 && args[4] is T5 && args[5] is T6 && args[6] is T7 && args[7] is T8 && args[8] is T9 && args[9] is T10 && args[10] is T11 && args[11] is T12;
    }

    public static bool Validate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
        object[] args,
        [NotNullWhen(true)]
        out T1? t1,
        [NotNullWhen(true)]
        out T2? t2,
        [NotNullWhen(true)]
        out T3? t3,
        [NotNullWhen(true)]
        out T4? t4,
        [NotNullWhen(true)]
        out T5? t5,
        [NotNullWhen(true)]
        out T6? t6,
        [NotNullWhen(true)]
        out T7? t7,
        [NotNullWhen(true)]
        out T8? t8,
        [NotNullWhen(true)]
        out T9? t9,
        [NotNullWhen(true)]
        out T10? t10,
        [NotNullWhen(true)]
        out T11? t11,
        [NotNullWhen(true)]
        out T12? t12,
        [NotNullWhen(true)]
        out T13? t13
    ) {
        t1 = default;
        t2 = default;
        t3 = default;
        t4 = default;
        t5 = default;
        t6 = default;
        t7 = default;
        t8 = default;
        t9 = default;
        t10 = default;
        t11 = default;
        t12 = default;
        t13 = default;

        if (args.Length != 13)
            return false;

        return args[0] is T1 && args[1] is T2 && args[2] is T3 && args[3] is T4 && args[4] is T5 && args[5] is T6 && args[6] is T7 && args[7] is T8 && args[8] is T9 && args[9] is T10 && args[10] is T11 && args[11] is T12 && args[12] is T13;
    }

    public static bool Validate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
        object[] args,
        [NotNullWhen(true)]
        out T1? t1,
        [NotNullWhen(true)]
        out T2? t2,
        [NotNullWhen(true)]
        out T3? t3,
        [NotNullWhen(true)]
        out T4? t4,
        [NotNullWhen(true)]
        out T5? t5,
        [NotNullWhen(true)]
        out T6? t6,
        [NotNullWhen(true)]
        out T7? t7,
        [NotNullWhen(true)]
        out T8? t8,
        [NotNullWhen(true)]
        out T9? t9,
        [NotNullWhen(true)]
        out T10? t10,
        [NotNullWhen(true)]
        out T11? t11,
        [NotNullWhen(true)]
        out T12? t12,
        [NotNullWhen(true)]
        out T13? t13,
        [NotNullWhen(true)]
        out T14? t14
    ) {
        t1 = default;
        t2 = default;
        t3 = default;
        t4 = default;
        t5 = default;
        t6 = default;
        t7 = default;
        t8 = default;
        t9 = default;
        t10 = default;
        t11 = default;
        t12 = default;
        t13 = default;
        t14 = default;

        if (args.Length != 14)
            return false;

        return args[0] is T1
            && args[1] is T2
            && args[2] is T3
            && args[3] is T4
            && args[4] is T5
            && args[5] is T6
            && args[6] is T7
            && args[7] is T8
            && args[8] is T9
            && args[9] is T10
            && args[10] is T11
            && args[11] is T12
            && args[12] is T13
            && args[13] is T14;
    }

    public static bool Validate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
        object[] args,
        [NotNullWhen(true)]
        out T1? t1,
        [NotNullWhen(true)]
        out T2? t2,
        [NotNullWhen(true)]
        out T3? t3,
        [NotNullWhen(true)]
        out T4? t4,
        [NotNullWhen(true)]
        out T5? t5,
        [NotNullWhen(true)]
        out T6? t6,
        [NotNullWhen(true)]
        out T7? t7,
        [NotNullWhen(true)]
        out T8? t8,
        [NotNullWhen(true)]
        out T9? t9,
        [NotNullWhen(true)]
        out T10? t10,
        [NotNullWhen(true)]
        out T11? t11,
        [NotNullWhen(true)]
        out T12? t12,
        [NotNullWhen(true)]
        out T13? t13,
        [NotNullWhen(true)]
        out T14? t14,
        [NotNullWhen(true)]
        out T15? t15
    ) {
        t1 = default;
        t2 = default;
        t3 = default;
        t4 = default;
        t5 = default;
        t6 = default;
        t7 = default;
        t8 = default;
        t9 = default;
        t10 = default;
        t11 = default;
        t12 = default;
        t13 = default;
        t14 = default;
        t15 = default;

        if (args.Length != 15)
            return false;

        return args[0] is T1
            && args[1] is T2
            && args[2] is T3
            && args[3] is T4
            && args[4] is T5
            && args[5] is T6
            && args[6] is T7
            && args[7] is T8
            && args[8] is T9
            && args[9] is T10
            && args[10] is T11
            && args[11] is T12
            && args[12] is T13
            && args[13] is T14
            && args[14] is T15;
    }

    public static bool Validate<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
        object[] args,
        [NotNullWhen(true)]
        out T1? t1,
        [NotNullWhen(true)]
        out T2? t2,
        [NotNullWhen(true)]
        out T3? t3,
        [NotNullWhen(true)]
        out T4? t4,
        [NotNullWhen(true)]
        out T5? t5,
        [NotNullWhen(true)]
        out T6? t6,
        [NotNullWhen(true)]
        out T7? t7,
        [NotNullWhen(true)]
        out T8? t8,
        [NotNullWhen(true)]
        out T9? t9,
        [NotNullWhen(true)]
        out T10? t10,
        [NotNullWhen(true)]
        out T11? t11,
        [NotNullWhen(true)]
        out T12? t12,
        [NotNullWhen(true)]
        out T13? t13,
        [NotNullWhen(true)]
        out T14? t14,
        [NotNullWhen(true)]
        out T15? t15,
        [NotNullWhen(true)]
        out T16? t16
    ) {
        t1 = default;
        t2 = default;
        t3 = default;
        t4 = default;
        t5 = default;
        t6 = default;
        t7 = default;
        t8 = default;
        t9 = default;
        t10 = default;
        t11 = default;
        t12 = default;
        t13 = default;
        t14 = default;
        t15 = default;
        t16 = default;

        if (args.Length != 16)
            return false;

        return args[0] is T1
            && args[1] is T2
            && args[2] is T3
            && args[3] is T4
            && args[4] is T5
            && args[5] is T6
            && args[6] is T7
            && args[7] is T8
            && args[8] is T9
            && args[9] is T10
            && args[10] is T11
            && args[11] is T12
            && args[12] is T13
            && args[13] is T14
            && args[14] is T15
            && args[15] is T16;
    }
}
