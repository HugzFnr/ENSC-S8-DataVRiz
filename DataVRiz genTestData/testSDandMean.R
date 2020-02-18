table <- read.table("charPersonRandomNoisyData.txt",header=TRUE,sep=",")

mean(table$sizeCm)
sd(table$sizeCm)
mean(table$massKg)
sd(table$massKg)
mean(table$age)
sd(table$age)