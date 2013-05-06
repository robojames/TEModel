clear all; clc;
M = csvread('T_Field.csv',1,0);

X = M(:,1); Y = M(:,2); T = M(:,3);

XNodes = linspace(min(X),max(X),max(size(M))/300);
YNodes = linspace(min(Y),max(Y),max(size(M))/300);
[z,x,y] = gridfit(X, Y, T, XNodes, YNodes);

surf(x,y,z); xlabel('X Position'); ylabel('Y Position'); zlabel('Temperature');