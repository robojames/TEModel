% Programmer:  James L. Armes
% Analytic Solution Modeling for TEM Model Validation
%clear all; clc;
A = 1.9516*10^-6;
L = 0.00132080;
k = 1.48;
I = 1.0;
sigma = 1*10^5;
x = Y1; %linspace(0,L);
Tc = 230;
Th = 250;


TTop = (I.^2.*L.*(L-x).*x+2.*A.^2.*k.*sigma.*(L.*Tc+(Th-Tc).*x));
TBot = 2.*A.^2.*k.*L.*sigma;

T_analytic_1 = TTop./TBot;


A = 1.9516*10^-6;
L = 0.00132080;
k = 1.48;
I = 2.0;
sigma = 1*10^5;
x = Y1;
Tc = 230;
Th = 250;


TTop = (I.^2.*L.*(L-x).*x+2.*A.^2.*k.*sigma.*(L.*Tc+(Th-Tc).*x));
TBot = 2.*A.^2.*k.*L.*sigma;

T_analytic_2 = TTop./TBot;


A = 1.9516*10^-6;
L = 0.00132080;
k = 1.48;
I = 3.0;
sigma = 1*10^5;
x = Y1;
Tc = 230;
Th = 250;


TTop = (I.^2.*L.*(L-x).*x+2.*A.^2.*k.*sigma.*(L.*Tc+(Th-Tc).*x));
TBot = 2.*A.^2.*k.*L.*sigma;

T_analytic_3 = TTop./TBot;


A = 1.9516*10^-6;
L = 0.00132080;
k = 1.48;
I = 4.0;
sigma = 1*10^5;
x = Y1;
Tc = 230;
Th = 250;


TTop = (I.^2.*L.*(L-x).*x+2.*A.^2.*k.*sigma.*(L.*Tc+(Th-Tc).*x));
TBot = 2.*A.^2.*k.*L.*sigma;

T_analytic_4 = TTop./TBot;


A = 1.9516*10^-6;
L = 0.00132080;
k = 1.48;
I = 5.0;
sigma = 1*10^5;
x = Y1;
Tc = 230;
Th = 250;


TTop = (I.^2.*L.*(L-x).*x+2.*A.^2.*k.*sigma.*(L.*Tc+(Th-Tc).*x));
TBot = 2.*A.^2.*k.*L.*sigma;

T_analytic_5 = TTop./TBot;

M1 = csvread('T_Mid_T_1.csv',1,0);
M2 = csvread('T_Mid_T_2.csv',1,0);
M3 = csvread('T_Mid_T_3.csv',1,0);
M4 = csvread('T_Mid_T_4.csv',1,0);
M5 = csvread('T_Mid_T_5.csv',1,0);

Y1 = M1(:,2);
T_numeric_1 = M1(:,3);

Y2 = M1(:,2);
T_numeric_2 = M2(:,3);

Y3 = M1(:,2);
T_numeric_3 = M3(:,3);

Y4 = M1(:,2);
T_numeric_4 = M4(:,3);

Y5 = M1(:,2);
T_numeric_5 = M5(:,3);

figure(1); hold on; grid on; xlabel('X Position, m'); ylabel('Temperature, K');
plot(x, T_analytic_1, 'k*');
plot(x, T_analytic_2, 'r*');
plot(x, T_analytic_3, '*');
plot(x, T_analytic_4, 'y*');
plot(x, T_analytic_5, 'c*');

plot(Y1, T_numeric_1, 'k^');
plot(Y2, T_numeric_2, 'r^');
plot(Y3, T_numeric_3, '^');
plot(Y4, T_numeric_4, 'y^');
plot(Y5, T_numeric_5, 'c^');

legend('Analytic, I=1','Analytic, I=2', 'Analytic, I=3', 'Analytic, I=4', 'Analytic, I=5', 'Numeric, I=1', 'Numeric, I=2', 'Numeric, I=3', 'Numeric, I=4', 'Numeric, I=5');

diff1 = abs(T_analytic_1 - T_numeric_1);
diff2 = abs(T_analytic_2 - T_numeric_2);
diff3 = abs(T_analytic_3 - T_numeric_3);
diff4 = abs(T_analytic_4 - T_numeric_4);
diff5 = abs(T_analytic_5 - T_numeric_5);

figure(2); hold on; grid on; xlabel('X Position, m'); ylabel('Temperature Error, K');
plot(Y1, diff1, 'k*'), plot(Y1,diff2, 'r*'), plot(Y1,diff3, '*'), plot(Y1,diff4, 'y^'), plot(Y1,diff5, 'c^');
legend('I=1', 'I=2', 'I=3', 'I=4', 'I=5');

