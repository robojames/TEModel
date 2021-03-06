clear all; clc;
M = csvread('T_Field_1.csv',1,0);

X = M(:,1); Y = M(:,2); T = M(:,3);


XNodes = linspace(min(X),max(X),max(size(M))/250);
YNodes = linspace(min(Y),max(Y),max(size(M))/250);

[z,x,y] = gridfit(X, Y, T, XNodes, YNodes);

surf(x,y,z, 'EdgeColor','none'); 
%surf(x,y,z); 
xlabel('X Position, m'); ylabel('Y Position, m'); zlabel('Temperature');
