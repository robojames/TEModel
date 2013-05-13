clear all; clc;
M = csvread('T_Field.csv',1,0);

X = M(:,1); Y = M(:,2); T = M(:,3);


XNodes = linspace(min(X),max(X),max(size(M)));
YNodes = linspace(min(Y),max(Y),max(size(M)));

[z,x,y] = gridfit(X, Y, T, XNodes, YNodes);

figure(1); hold on;
subplot(1,2,1);

surf(x,y,z, 'EdgeColor','none'); xlabel('X Position, m'); ylabel('Y Position, m'); zlabel('Temperature');

for i = 1:max(size(X))
   T_exact(i) = Exact(X(i), Y(i));
end

subplot(1,2,2);
[z2,x2,y2] = gridfit(X, Y, T_exact, XNodes, YNodes);
surf(x2,y2,z2, 'EdgeColor','none'); xlabel('X Position, m'); ylabel('Y Position, m');
zlabel('Temperature, K');

