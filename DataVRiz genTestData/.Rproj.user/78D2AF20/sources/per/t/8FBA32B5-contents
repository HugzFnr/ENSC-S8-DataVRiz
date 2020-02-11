#if mass was correlated to size for people around 50 yo
sizeCm<-rnorm(n=100,mean=175,sd=15)
massKg<-sizeCm*0.5
age<-rnorm(n=100,mean=50,sd=10)

massKgNoisy<-jitter(massKg,factor=1000)

cleanData<-data.frame(sizeCm,massKg,age)

noisyData<-data.frame(sizeCm,massKgNoisy,age)

write.table(cleanData,"charPersonRandomData.txt",sep=",")

write.table(noisyData,"charPersonRandomNoisyData.txt",sep=",")