using ActUtlType64Lib;

namespace Mes.PlcSmokeTest;

internal class Program
{
    [STAThread]
    static void Main()
    {
        Console.WriteLine(
            "=================================="
        );

        Console.WriteLine(
            " MX Component .NET 8 Smoke Test"
        );

        Console.WriteLine(
            "=================================="
        );

        Console.WriteLine();


        ActUtlType64Class? plc = null;


        try
        {
            plc =
                new ActUtlType64Class();


            plc.ActLogicalStationNumber = 1;


            // ==============================
            // OPEN
            // ==============================

            int result =
                plc.Open();


            Console.WriteLine(
                $"Open() 결과 : 0x{result:X8}"
            );


            if (result != 0)
            {
                Console.WriteLine(
                    "PLC 연결 실패"
                );

                return;
            }


            Console.WriteLine(
                "PLC 연결 성공"
            );

            Console.WriteLine();


            // ==============================
            // D100 읽기
            // ==============================

            result =
                plc.GetDevice(
                    "D100",
                    out int before
                );


            Console.WriteLine(
                $"GetDevice(D100) 결과 : 0x{result:X8}"
            );

            Console.WriteLine(
                $"D100 기존 값         : {before}"
            );


            if (result != 0)
            {
                return;
            }


            Console.WriteLine();


            // ==============================
            // D100 쓰기
            // ==============================

            const int testValue = 1234;


            result =
                plc.SetDevice(
                    "D100",
                    testValue
                );


            Console.WriteLine(
                $"SetDevice(D100, {testValue}) 결과 : 0x{result:X8}"
            );


            if (result != 0)
            {
                return;
            }


            Console.WriteLine();


            // ==============================
            // D100 재확인
            // ==============================

            result =
                plc.GetDevice(
                    "D100",
                    out int after
                );


            Console.WriteLine(
                $"GetDevice(D100) 결과 : 0x{result:X8}"
            );

            Console.WriteLine(
                $"D100 변경 후 값       : {after}"
            );


            Console.WriteLine();


            if (result == 0 &&
                after == testValue)
            {
                Console.WriteLine(
                    "✅ .NET 8 ↔ MX Component 통신 성공"
                );
            }
            else
            {
                Console.WriteLine(
                    "❌ D100 값 검증 실패"
                );
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine();

            Console.WriteLine(
                $"예외 발생: {ex}"
            );
        }
        finally
        {
            if (plc != null)
            {
                try
                {
                    int closeResult =
                        plc.Close();


                    Console.WriteLine();

                    Console.WriteLine(
                        $"Close() 결과 : 0x{closeResult:X8}"
                    );
                }
                catch
                {
                }
            }
        }


        Console.WriteLine();

        Console.WriteLine(
            "ENTER를 누르면 종료합니다."
        );

        Console.ReadLine();
    }
}