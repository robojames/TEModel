function [Texact] = Exact(x,y)
% Theta
T1 = 250;
T2 = 350;
n_iter = 200;
W = 1.32080*10^-3;
L = 1.397*10^-3;

        for i = 1:n_iter
            
            part1 = (((-1)^(i+1))+1)/i;
            part2 = sin((i*pi*x/L));
            part3 = sinh((i*pi*y/L));
            part4 = sinh(i*pi*W/L);
            
            temp_theta(i) = part1*part2*(part3/part4);
            
            
        end

 Texact=sum(temp_theta)*(2/pi)*(T2-T1) + T1;
 