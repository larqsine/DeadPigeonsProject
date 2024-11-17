#!/bin/bash
dotnet ef dbcontext scaffold \
 "Server=localhost;Database=DBdp;User Id=DBuser;Password=DBpass;" \
 Npgsql.EntityFrameworkCore.PostgreSQL \
 --output-dir ../DataAccess/Models\
 --context-dir . \
 --context dbContext  \
 --no-onconfiguring \
 --data-annotations \
 --force