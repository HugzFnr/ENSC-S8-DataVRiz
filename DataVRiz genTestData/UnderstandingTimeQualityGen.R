#if mass was correlated to size for people around 50 yo
timeSpentTryingMins<-rnorm(n=745,mean=15,sd=5)

qualityOfThePresentations<-rnorm(n=745,mean=20,sd=15)
qualityOfThePresentations<-jitter(qualityOfThePresentations,factor=10)


understandingOfTheProject<- timeSpentTryingMins*0.2 + qualityOfThePresentations*0.3
understandingOfTheProject<-jitter(understandingOfTheProject,factor=100)

availableCognitiveCapacityPossibilities<-factor(c("Low","Medium","High","Surhuman"))
availableCognitiveCapacity<-c(1:745)
k<-1
for (val in timeSpentTryingMins) {
  if (val < 11) { availableCognitiveCapacity[k]<-"Low"
} else if (val<15) { availableCognitiveCapacity[k]<-"Medium"
} else if (val<18) { availableCognitiveCapacity[k]<-"High"
} else { availableCognitiveCapacity[k]<-"Surhuman"
}
  k=k+1
}

df<-data.frame(understandingOfTheProject,timeSpentTryingMins,qualityOfThePresentations,availableCognitiveCapacity)

write.csv(df,"UnderstandingTimeQuality.csv")