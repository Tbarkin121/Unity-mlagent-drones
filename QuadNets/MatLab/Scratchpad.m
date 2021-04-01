% close all
target = 0
range = -25:0.1:25
sensitivity = .5;
reward = exp(-sensitivity*abs(target-range))

plot(range,reward)

%%
val = -10 : .1 : 10 
scale = 0.25
val = val * scale

valreturn = val ./ (1 + abs(val));

plot(val,valreturn)

%%
a = 1;
b = 1;
c = 1;

a*exp(-((x-b)^2)/(2*c^2))


%%
target = 0
actual = 0:10
Delta = abs(target - actual)
if(Delta < 0)
    Delta = 0
end
if(Delta>target)
    Delta = target
end

output = (1-(Delta/target).^2).^2

plot(actual, output)
hold on