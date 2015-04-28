# Log4NetExtensions
Currently only a BurstFilter for log4net

Example configuration

    <log4net>
        <appender name="SMTPAppender" type="log4net.Appender.SmtpAppender,log4net">
          <bufferSize value="1"/>
          <authentication value="Basic" />
          <to value="to@example.com" />
          <from value="from@example.com" />
          <subject value="Automatmail: system X is totally broken, please fix" />
          <smtpHost value="smtp.example.com" />
          <username value="username"/>
          <password value="password"/>
          <lossy value="false" />
          <threshold value="ERROR"/>
          <filter type="Kentor.Log4NetExtensions.BurstFilter">
		    <!-- Allow two mails a minute on average, with bursts up to 20 mails -->
            <BurstLength value="00:10:00"/>
            <BurstSize value="20"/>
          </filter>
          <layout type="log4net.Layout.PatternLayout">
            <conversionPattern value="%property{log4net:HostName}%newline%date user:%aspnet-request{AUTH_USER} %-5level %logger %newline %message%newline%exception%newline%newline"/>
          </layout>
        </appender>
        <root>
          <level value="ALL" />
          <appender-ref ref="SMTPAppender" />
        </root>
    </log4net>