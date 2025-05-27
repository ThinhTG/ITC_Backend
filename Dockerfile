# Use the official Microsoft SQL Server image
FROM mcr.microsoft.com/mssql/server:2022-latest

# Set environment variables
ENV ACCEPT_EULA=Y
ENV MSSQL_SA_PASSWORD=GiaThinh123@
ENV MSSQL_PID=Express

# Create a directory for database files
RUN mkdir -p /var/opt/mssql/data

# Create a directory for the initialization script
WORKDIR /usr/src/app

# Copy initialization script
COPY ./init.sql ./

# Change permissions
USER root
RUN chmod +x ./init.sql
USER mssql

# Expose SQL Server port
EXPOSE 1433

# Start SQL Server and run the initialization script
CMD /opt/mssql/bin/sqlservr & sleep 30s && /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD -i ./init.sql & wait $! 