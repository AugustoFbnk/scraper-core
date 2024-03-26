FROM mcr.microsoft.com/dotnet/runtime:7.0.3-bullseye-slim-amd64 AS base
ENV TZ=America/Sao_Paulo
ENV LANG pt-BR
ENV LANGUAGE pt-BR
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/deb.debian.org/ftp.br.debian.org/g' /etc/apt/sources.list
RUN apt update
RUN apt install bash-completion bind9-utils net-tools vim tzdata telnet hostname htop -y
WORKDIR /scraper
COPY servicebin /scraper/
COPY configureBkService.sh /scraper/
RUN chmod +x configureBkService.sh
RUN chmod +x Scraper.BackgroundTasks
RUN apt-get update \
    && apt-get install -y wget gnupg \
    && wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - \
    && sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list' \
    && apt-get update \
    && apt-get install -y google-chrome-stable fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-kacst fonts-freefont-ttf libxss1 \
      --no-install-recommends \
    && rm -rf /var/lib/apt/lists/*
CMD /bin/bash configureBkService.sh
