% Programmer:  James L. Armes
% Analytic Solution Modeling for TEM Model Validation
clear all; clc;
A = 1.9516*10^-6;
L = 0.00132080;
k = 1.48;
I = 3.00;
sigma = 1*10^5;
x = linspace(0,L);
Tc = 230;
Th = 250;


TTop = (I.^2.*L.*(L-x).*x+2.*A.^2.*k.*sigma.*(L.*Tc+(Th-Tc).*x));
TBot = 2.*A.^2.*k.*L.*sigma;

T_analytic = TTop./TBot;

M = csvread('T_Mid_T.csv',1,0);
Y = M(:,2);
T_numeric = M(:,3);
figure(2); hold on;
%plot(x, T_analytic,'r*'); xlabel('X Position, m'); ylabel('Temperature, K');
plot(Y, T_numeric, 'k^'); legend('Analytic','Numeric');