#if mass was correlated to size for people around 50 yo
timeSpentTryingMins<-rnorm(n=745,mean=15,sd=5)

qualityOfThePresentations<-rnorm(n=745,mean=20,sd=15)
qualityOfThePresentations<-jitter(qualityOfThePresentations,factor=10)


understandingOfTheProject<- timeSpentTryingMins*0.2 + qualityOfThePresentations*0.3
understandingOfTheProject<-jitter(understandingOfTheProject,factor=100)

test<-data.frame(understandingOfTheProject,timeSpentTryingMins,qualityOfThePresentations)

availableCognitiveCapacityPossibilities<-factor(c("Low","Medium","High","Surhuman"))
availableCognitiveCapacity<-c()
for (val in timeSpentTryingMins)
{
  if (val < 11) {
    availableCognitiveCapacity=availableCognitiveCapacityPossibilities[1]
} else if (val<15) { availableCognitiveCapacity=availableCognitiveCapacityPossibilities[2]
} else if (val<18) { availableCognitiveCapacity=availableCognitiveCapacityPossibilities[3]
}else { availableCognitiveCapacity=availableCognitiveCapacityPossibilities[4]
}
}



noisyData<-data.frame(sizeM,massKgNoisy,averageKcalEaten)
write.csv(noisyData,"10kpointscharPersonNoisy.csv")