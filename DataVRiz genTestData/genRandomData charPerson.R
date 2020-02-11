#if mass was correlated to size for people around 50 yo
sizeCm<-rnorm(n=100,mean=175,sd=15)
massKg<-sizeCm*0.5
age<-rnorm(n=100,mean=50,sd=10)

table<-data.frame(sizeCm,massKg,age)

write.table(table,"charPersonRandomData.txt",sep=",")