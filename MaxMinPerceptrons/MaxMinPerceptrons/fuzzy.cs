using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaxMinPerceptrons
{
    class fuzzy
    {
        int correct;
        int Incorrect;
        // 정확성과 부정확성 수 알아내기

        // layer는 한 패턴에 

        //대한 출력값을 표현한 것이기 때문에 (문제점)
        // 모든 패턴에 대한 출력값을 가지는 배열로 가져와야함
        // 하지만 교수님 소스에는 모든 패턴에 대한 출력값을 가져오는 배열이 없기 때문에
        // 직접 수정하여 넣어주어야함. // 수정..완료 ?
        public void Getcorrect(double[,] layer, double[,] target) // 출력값은 [패턴수, 출력노드 수] 배열로 받아와야지!! ?.?
        {
            correct=0; // 정확성 수
            Incorrect=0; // 부정확성 수
            int i; int j; // for문반복문 변수
            double e = 0.1; // 수치
            for (i=0; i< 5; i++) //5개의 패턴
            {
                for (j = 0; j<3; j++) // 3개의 출력
                {
                    double value = Math.Abs(layer[i,j] - target[i, j]); // Math.Abs() 빼먹음!! 수정완료
                    if (value <= e)
                        correct++;
                    else
                        Incorrect++;
                }
            }
            //Console.WriteLine(correct + " " + Incorrect);
        }
        //퍼지소속함수 그리기 > ?

        double lim;
        double mid;
        double low;
        double High;
        // 소속함수의 경계값 설정
        public void Getborderline(int Maxpatten, int Maxoutput, int Maxinput)
        {
            lim = Maxpatten * Maxoutput; // 최대패턴수 * 최대출력수 //정확성의한계치??(최대로 가지는 정확성값) 확인완료
            mid = lim / 2; // 중간값
            low = Math.Log(Maxinput + Maxoutput, 2.0);  // Math.Log(input + output, 2.0); 요거는 교수님이 설명안해준 것이라 일단 요 식으로 사용하면됨 mid / 2; 수정완료
            High = lim - low; // 논문식에 lim-low 로 되어있음!! 수정완료

            //lim는15  High는9.3275746580285  mid는7.5  low는5.6724253419715
            //Console.WriteLine("lim는" + lim + "  High는" + High + "  mid는" + mid + "  low는" + low);
        }

        double inferredPss;

        // 퍼지추론
        public void getInference()
        {
            inferredPss = 0.0;
            if (correct <= low) // 7 <= 5.6724
            {
                inferredPss = 0.75;
                //Console.Write(correct + "는   " + low + " low 이므로 학습률 0.75로 조절합니다.");
            }
            else if (correct > low && correct <= High) // 7 > 5.6724 이고 7 <= 13.1724
            {
                inferredPss = 0.5;
                //Console.Write(correct + "는   " + mid + " mid 이므로 학습률 0.5로 조절합니다.");
            }
            else if (correct > High) // 7 > 13.1724
            {
                inferredPss = 0.25;
                //Console.Write(correct + "는   " + High + " High 이므로 학습률 0.25로 조절합니다.");
            }
            Console.WriteLine();
        }

        double belong1;
        double belong2;
        // 소속도 구하기
        public void getbelong()
        {
            belong1 = 0;
            belong2 = 0;
            if (correct > low && correct < mid)
            {
                belong1 = (mid - correct) / (mid - low);
                belong2 = 1 - belong1;
                //Console.WriteLine("1   " + belong2);
            }
            else if (correct > mid && correct < High)
            {
                belong1 = (High - correct) / (High - mid);
                belong2 = 1 - belong1;
                //Console.WriteLine("2   " + belong2);
            }
            else if (correct == (int)mid)
            {
                belong2 = 1;
               // Console.WriteLine("3   " + belong2);
            }
            else
            {
                belong2 = 1;
                //Console.WriteLine("4   " + belong2);
            }
        }
        //        분자     부모   학습된 학습률
        double Molecular, Fennel, learning_rate;
        public double Beefudge()
        {
            Molecular = 0.0; Fennel = 0.0; learning_rate = 0.0;
            Molecular = belong2 * inferredPss; Console.WriteLine("belong2 : " + belong2 + "inferredPss : " + inferredPss + "Molecular : " + Molecular);
            Fennel = belong1 + belong2; //Console.WriteLine("Fennel : " + Fennel);
            learning_rate = MaxMinWeight(Molecular / Fennel);
            Console.WriteLine("learning_rate : " + learning_rate);
            return learning_rate;
        }

        public double MaxMinWeight(double n)
        {
            return Math.Min(Math.Max(0.0, n), 1.0);
        }

    }
}
