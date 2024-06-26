# NATS JetStream Message Queue

This README provides instructions for setting up and running a C# console app to test NATS JetStream using Docker, NATS client, Grafana, and Prometheus. The focus of this test is to evaluate deduplication functionality.

To get started, follow these steps:

1. Install Docker on your machine if you haven't already.
2. Clone the repository containing the C# console app.
3. Open a terminal and navigate to the root directory of the cloned repository.
4. Start the required services by running the following command:

    ```
    docker-compose up -d
    ```

    This will start the NATS server, Grafana, and Prometheus containers.

5. Build the C# console app by running the appropriate build command for your development environment.

6. Run the console app and observe the deduplication functionality in action.

    The app will publish messages to the NATS JetStream and verify that duplicate messages are not processed multiple times.

7. Monitor the metrics and visualize the data in Grafana by accessing the Grafana dashboard at `http://localhost:3000` in your web browser.

8. Analyze the deduplication results and make any necessary adjustments to your application logic.


