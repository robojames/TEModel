% Programmer:  James L. Armes
% Analytic Solution Modeling for TEM Model Validation
clear all; clc;
A = 1.4*10^-6;
L = 0.0013081;
k = 1.48;
alpha = 2*10^-4;
I = 4.0;
sigma = 1*10^5;
x = linspace(0,0.0013081);
Tc = 230;
Th = 300;


TTop = (I.^2.*L.*(L-x).*x+2.*A.^2.*k.*sigma.*(L.*Tc+(Th-Tc).*x));
TBot = 2.*A.^2.*k.*L.*sigma;

T=TTop./TBot;
plot(T);