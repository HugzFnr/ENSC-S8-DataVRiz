#if mass was correlated to size for people around 50 yo
sizeM<-rnorm(n=10000,mean=1.75,sd=0.15)
massKg<-sizeM*50
massKgNoisy<-jitter(massKg,factor=100)
averageKcalEaten<-rnorm(n=10000,mean=2000,sd=50) + massKgNoisy*5

noisyData<-data.frame(sizeM,massKgNoisy,averageKcalEaten)
write.csv(noisyData,"10kpointscharPersonNoisy.csv")