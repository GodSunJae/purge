using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaxMinPerceptrons
{
    class MaxMinClass
    {
        const int MAX_PATTERN = 5; // 최대 패턴수
        const int DIM = 48; // 최대 입력수
        const int L_2_NODE = 48;
        const int L_3_NODE = 3; // 출력 수
        const int OUTPUT = L_3_NODE;
        const int INPUT = DIM; 
        
        int N_PATTERN = MAX_PATTERN; // 최대 패턴수
        int Max_Iteration = 1000;


        // 임의에 5개패턴을 입력
        static double[,] pattern = new double[5, 48] {{1.0,1.0,1.0,1.0,1.0,1.0,
                                                       0.0,1.0,1.0,1.0,0.0,0.0,
                                                       0.0,1.0,1.0,1.0,0.0,0.0,
                                                       0.0,1.0,1.0,1.0,0.0,0.0,
                                                       0.0,1.0,1.0,1.0,0.0,0.0,
                                                       0.0,1.0,1.0,1.0,0.0,0.0,
                                                       0.0,1.0,1.0,1.0,0.0,0.0,
                                                       1.0,1.0,1.0,1.0,1.0,1.0},

                                                      {1.0,1.0,1.0,1.0,1.0,0.0,
                                                       1.0,0.0,0.0,0.0,0.0,1.0,
                                                       1.0,0.0,0.0,0.0,0.0,1.0,
                                                       1.0,0.0,0.0,0.0,0.0,1.0,
                                                       1.0,1.0,1.0,1.0,1.0,0.0,
                                                       1.0,0.0,0.0,0.0,0.0,0.0,
                                                       1.0,0.0,0.0,0.0,0.0,0.0,
                                                       1.0,1.0,0.0,0.0,0.0,0.0},

                                                      {0.0,0.0,1.0,1.0,0.0,0.0,
                                                       0.0,1.0,0.0,0.0,1.0,0.0,
                                                       0.0,1.0,0.0,0.0,0.0,1.0,
                                                       0.0,1.0,1.0,1.0,1.0,1.0,
                                                       0.0,0.0,0.0,0.0,0.0,1.0,
                                                       0.0,1.0,0.0,0.0,1.0,0.0,
                                                       0.0,1.0,0.0,0.0,1.0,1.0,
                                                       0.0,1.0,1.0,1.0,1.0,1.0},

                                                      {1.0,0.0,0.0,0.0,0.0,1.0,
                                                       1.0,1.0,1.0,1.0,1.0,1.0,
                                                       0.0,0.0,1.0,1.0,0.0,0.0,
                                                       0.0,0.0,1.0,1.0,0.0,0.0,
                                                       0.0,0.0,1.0,1.0,0.0,0.0,
                                                       0.0,0.0,1.0,1.0,0.0,0.0,
                                                       0.0,0.0,1.0,1.0,0.0,0.0,
                                                       0.0,0.0,0.0,0.0,0.0,0.0},

                                                      {0.0,1.0,1.0,1.0,1.0,1.0,
                                                       0.0,0.0,0.0,0.0,0.0,1.0,
                                                       0.0,0.0,0.0,0.0,1.0,0.0,
                                                       0.0,0.0,0.0,1.0,0.0,0.0,
                                                       0.0,0.0,1.0,0.0,0.0,0.0,
                                                       0.0,1.0,0.0,0.0,0.0,0.0,
                                                       1.0,1.0,1.0,1.0,1.0,0.0,
                                                       0.0,0.0,0.0,0.0,0.0,0.0}};
        // 목표값
        double[,] target = new double[5, 3] { { 1.0, 0.0, 0.0 },
                                              { 0.0, 1.0, 0.0 },
                                              { 0.0, 0.0, 1.0 }, 
                                              { 1.0, 1.0, 0.0 }, 
                                              { 0.0, 1.0, 1.0 } };

       

        double[,] weight = new double[OUTPUT, INPUT];
        double[,] dweight = new double[OUTPUT, INPUT];
        double[,] old_dweight = new double[OUTPUT, INPUT];
        double[] min = new double[INPUT];
        double[] max = new double[OUTPUT];
        double[] bias = new double[OUTPUT];
        double[] dbias = new double[OUTPUT];
        double[] old_dbias = new double[OUTPUT];
        double[,] layer = new double[MAX_PATTERN, OUTPUT]; // 실제출력값?
        //double[,] layer2 = new double[MAX_PATTERN, OUTPUT];
        double Lrate1 = 0.5, Lrate2 = 0.4;
       // double Lrate1 = 0.0000000001, Lrate2 = 0.3000010000000;
        double Momentum1 = 0.3, Momentum2 = 0.2;
        double Ecrit = 0.001;
        int nepoch;
        double pss, tss;
        StringBuilder sb = new StringBuilder(); // string형을 단일쓰레드일때 읽기쓰기에 속도문제를 해결해줌


        // 1단계 임의값 넣기
        public void Initialize_weight()
        {
            //  가중치와 바이어스값을 임의값을 넣어준다. 
            int i, j;
            Random rr = new Random();
            for (i = 0; i < OUTPUT; i++)
            {
                for (j = 0; j < INPUT; j++)
                {   // 임의값으로 가중치를 구함                          
                    weight[i, j] = ((double)(rr.Next(10000)) / 10000.0 / 2) + 0.5;
                }
                //
                bias[i] = ((double)(rr.Next(10000)) / 10000.0 / 2) + 0.5;
            }
        }

        // 2단계  가중치, 방향성, 변화 등 구함
        public void activate(int index)
        {
            int i, j;
            double wdiff, bdiff;
            //초기화
            for (i = 0; i < OUTPUT; i++) // 0에서 3까지
                max[i] = 0.0;

            for (i = 0; i < OUTPUT; i++) // 0에서 3까지
            {
                for (j = 0; j < INPUT; j++) // 0에서 48까지
                {
                    // 가중치값과 입력값을 비교하여 작은값을 넣음 // 비교해서 공통된부분넣어야하는데 여기서 작은값으로함
                    if (pattern[index, j] > weight[i, j])
                        min[j] = weight[i, j]; // 가중치가 작네? 넣고
                    else
                        min[j] = pattern[index, j]; // 입력이 더 작네? 넣고

                    // 이 민값이 맥스보다 큰지 확인해서 크다면 맥스에다가 넣음
                    if (max[i] < min[j]) max[i] = min[j];
                }
                // 맥스값이랑 바이어스값 비교해서 큰값 을 layer에 넣는다.  // 바이어스는 생각한값(?)이라서 반영을 넓게해야하므로 둘중 최대값을 구해야하는거임
                layer[index, i] = (max[i] > bias[i]) ? max[i] : bias[i];
                //layer2[index, i] = layer[i];
            }

            for (i = 0; i < OUTPUT; i++) // 0에서 3까지
            {
                for (j = 0; j < INPUT; j++) // 0에서 48까지
                {
                    wdiff = (layer[index, i] == weight[i, j]) ? 1.0 : 0.0; // wdiff는             가중치 방향성   // 이 수치가 목표 수치 ?쪽으로 잘 가고있는지 그런 방향성을 표현을 함 만약에 잘안가고있다면 잘가게끔해줘야댐
                    dweight[i, j] += wdiff * (target[index, i] - layer[index, i]); //delta weight 가중치 변화률   // 현재 수치와 이전수치 (?) 와 얼마나 변화를 하였는지 대한 수치
                }
                bdiff = (layer[index, i] == bias[i]) ? 1.0 : 0.0; //      바이어스 방향성
                dbias[i] += bdiff * (target[index, i] - layer[index, i]);  // 바이어스 변화률
            }
        }
        // 3단계 목표값과 출력값 사이의 오류값을 계산
        public void compute_pss(int index, double learning_rate)
        {
            int i;
           // double error;
           // pss = 0.0;
            for (i = 0; i < OUTPUT; i++) // 0에서 3까지
            {
                // 출력값 - 목표값해서 에러값을 구하고
                //error = layer[index, i] - target[index, i];
                // 수식에 나와있는대로 에러값을 제곱한다.
                //pss += error * error; // 수식에 따라 제곱후에 학습률을 더한다. // 미분 
                pss = learning_rate;
                //Console.WriteLine(pss);
                Lrate1 = pss; 
                Lrate2 = pss; 
                Momentum1 = 1 - pss;
                Momentum2 = 1 - pss;
                Console.WriteLine("Momentum1 : " + Momentum1);
            }
        }
        // 4단계 가중치와 바이어스 항을 조정한다.
        public void change_weight()
        {
            int i, j;
            for (i = 0; i < OUTPUT; i++)
            {
                for (j = 0; j < INPUT; j++)
                {
                    // 모멘텀 : 이전가중치에 얼마나 반영할것인지 대한 수치

                    //  현재 가중치 + 학습률 * 현재 가중치 변화률 + 모멘덤 * 이전가중치 변화률
                    weight[i, j] += Lrate1 * dweight[i, j] + Momentum1 * old_dweight[i, j];
                    if (weight[i, j] < 0.0) weight[i, j] = 0.0;  // 0보다 아래면 0
                    if (weight[i, j] >= 1.0) weight[i, j] = 1.0; // 1보다 높으면 1
                    old_dweight[i, j] = dweight[i, j]; // 현재가중치 변화률 이전 가중치변화에다 넣음
                    dweight[i, j] = 0.0; // 초기화
                }
                //  현재 가중치 + 학습률 * 현재 바이어스 변화률 + 모멘텀 * 이전가중치 변화률
                bias[i] += Lrate2 * dbias[i] + Momentum2 * old_dbias[i];
                if (bias[i] < 0.0) bias[i] = 0.0;  // 0보다 아래면 0
                if (bias[i] >= 1.0) bias[i] = 1.0; // 1보다 높으면 1
                old_dbias[i] = dbias[i]; // 현재 바이어스 변화률 이전 바이어스 변화에다가 넣음
                dbias[i] = 0.0; // 초기화
            }
        }
        // 5단계 모든 패턴이 학습 패턴에 대하여 부정확성의 수가 0보다 크면 < 단계 2 >로 가서 반복한다.
        public void Recognition()
        {
            int i, j;
            sb.AppendLine();
            sb.AppendLine("Recognition result");
            for (i = 0; i < N_PATTERN; i++)
            {
                //activate(i);
                for (j = 0; j < OUTPUT; j++)
                {
                    sb.AppendLine("Target[" + i + "][" + j + "]= " + target[i, j] + ", computed[" + j + "]= " + layer[i,j]);
                    if (((target[i, j] - layer[i,j]) > 0.5) || ((target[i, j] - layer[i,j]) < -0.5)) // 오차가 0.5이상 차이가 난다면 
                        sb.AppendLine("***"); // 제대로 추출하지몬하였으니 이렇게 출력
                }
            }
        }

        public String run()
        {
            fuzzy fuzzy = new fuzzy();
            fuzzy.Getborderline(MAX_PATTERN, OUTPUT, INPUT);
            int i;
            Initialize_weight(); // 처음에는 임의에 가중치와 바이어스값이 들어감
            sb.AppendLine("Learning Process"); 
            nepoch = 0; // 몇번이나 들어가는지 대해 초기화를 시켜줌
            do
            {
                nepoch++; // 학습횟수증가
                tss = 0.0;
                for (i = 0; i < N_PATTERN; i++) // 0에서 5까지
                { 
                    activate(i);
                    fuzzy.Getcorrect(layer, target);
                    fuzzy.getInference();
                    fuzzy.getbelong();
                    compute_pss(i, fuzzy.Beefudge());
                    tss += pss; // 
                }
                change_weight(); 
                sb.AppendLine("[" + nepoch + "] : TSS = " + tss + "\n");
            } while (tss > Ecrit); // 
            Recognition();
            
            return sb.ToString();
        }
    }
}
