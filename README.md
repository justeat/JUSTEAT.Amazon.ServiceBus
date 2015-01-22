JUSTEAT.Amazon.ServiceBus
==========
_A simple service bus implementation for Amazon Web Services using Amazon Simple Notification Service and Amazon Simple Queue Service_

---

* Introduction
* Installation
* Getting Started
* Contributing
* Copyright

JUSTEAT.Amazon.ServiceBus is a very simple service bus implementation for Amazon Web Services. Currently Amazon Simple Queue Service (Amazon SQS) is the only supported bus with the ability to publish messages to the bus either via Amazon SQS or Amazon SNS. JUSTEAT.Amazon.ServiceBus provides both a configurable concurrent listener for the bus. JUSTEAT.Amazon.ServiceBus manages the instantation of Amazon SNS Topics, Amazon SQS Queues and Amazon SNS Subscriptions where required. 

## Installation

Pre-requisites: The project is built in .net v4.5.

* From source: https://github.com/justeat/JUSTEAT.Amazon.ServiceBus
* By hand: https://www.nuget.org/packages/JUSTEAT.Amazon.ServiceBus

Via NuGet:

		PM> Install-Package JUSTEAT.Amazon.ServiceBus


## Getting Started

For a fully worked example please consult the JUSTEAT.Amazon.ServiceBusExample project.  You will need to edit the app.config to either provide your AWSProfile or your AWSAccessKey and AWSSecretKey.

Setup consists of 3 parts.  

1. Choose and construct the publisher
2. Choose and construct the reciever
3. Construct the service bus

One this is complete you will be able to call StartReceiving to recieve messages from the bus and Publish to publish messages to the bus

## Contributing

If you find a bug, have a feature request or even want to contribute an enhancement or fix, please follow the contributing guidelines included in the repository.


## Copyright

Copyright © JUST EAT PLC 2014