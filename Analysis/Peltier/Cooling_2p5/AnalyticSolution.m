% Programmer:  James L. Armes
% Analytic Solution Modeling for TEM Model Validation
clear all; clc;
A = 1.9516*10^-6;
L = 0.00132080;
k = 1.48;
I = 1.00;
sigma = 1*10^5;
x = linspace(0,L);
Th = 250;
J=I/A;
rhoe = 1/sigma;
alph = 2.0*10^-4;


T_analytic_1 = Th - (alph*J*Th*x)/k;



A = 1.9516*10^-6;
L = 0.00132080;
k = 1.48;
I = 2.00;
sigma = 1*10^5;
x = linspace(0,L);
Th = 250;
J=I/A;
rhoe = 1/sigma;
alph = 2.0*10^-4;


T_analytic_2 = Th - (alph*J*Th*x)/k;


A = 1.9516*10^-6;
L = 0.00132080;
k = 1.48;
I = 3.00;
sigma = 1*10^5;
x = linspace(0,L);
Th = 250;
J=I/A;
rhoe = 1/sigma;
alph = 2.0*10^-4;


T_analytic_3 = Th - (alph*J*Th*x)/k;


A = 1.9516*10^-6;
L = 0.00132080;
k = 1.48;
I = 4.00;
sigma = 1*10^5;
x = linspace(0,L);
Th = 250;
J=I/A;
rhoe = 1/sigma;
alph = 2.0*10^-4;


T_analytic_4 = Th - (alph*J*Th*x)/k;

A = 1.9516*10^-6;
L = 0.00132080;
k = 1.48;
I = 5.00;
sigma = 1*10^5;
x = linspace(0,L);
Th = 250;
J=I/A;
rhoe = 1/sigma;
alph = 2.0*10^-4;


T_analytic_5 = Th - (alph*J*Th*x)/k;

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