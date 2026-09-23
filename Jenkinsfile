pipeline {
    agent any

    parameters {
        booleanParam(
            name: 'RUN_SONAR',
            defaultValue: false,
            description: 'Enable SonarQube analysis after the SonarQube-Server installation is configured in Jenkins.'
        )
    }

    environment {
        GITHUB_REPO = 'https://github.com/khanglehuynhofficial/Hospital-Information-System-HIS.git'
        CREDENTIALS_ID = 'his-github-auth'
        SLACK_CREDENTIALS_ID = 'slack-token-secret'
        SLACK_CHANNEL = '#his-devops-alerts'
        SCANNER_HOME = '/opt/sonar-scanner'
    }

    stages {
        stage('1. Checkout Source Code') {
            steps {
                echo '=== STEP 1: PULLING LATEST CODE FROM GITHUB ==='
                checkout([$class: 'GitSCM',
                    branches: [[name: '*/develop']],
                    extensions: [],
                    userRemoteConfigs: [[credentialsId: "${CREDENTIALS_ID}", url: "${GITHUB_REPO}"]]
                ])
            }
        }

        stage('2. Build .NET') {
            steps {
                echo '=== STEP 2: BUILDING .NET PROJECT ==='
                sh '''
                    set -eu
                    trap 'status=$?; echo "BUILD SHELL EXIT CODE: $status"' EXIT
                    export DOTNET_CLI_TELEMETRY_OPTOUT=1
                    export DOTNET_NOLOGO=true

                    if ! command -v dotnet >/dev/null 2>&1 || ! dotnet --version >/dev/null 2>&1; then
                        if ! command -v curl >/dev/null 2>&1; then
                            echo 'ERROR: Jenkins agent has neither dotnet nor curl to install the .NET SDK.'
                            exit 127
                        fi

                        echo 'The Jenkins agent has no usable dotnet command; installing the .NET 8 SDK in the workspace.'
                        rm -rf "$WORKSPACE/.dotnet"
                        mkdir -p "$WORKSPACE/.dotnet"
                        curl -fsSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin \
                            --channel 8.0 \
                            --install-dir "$WORKSPACE/.dotnet" \
                            --no-path
                        export DOTNET_ROOT="$WORKSPACE/.dotnet"
                        export PATH="$DOTNET_ROOT:$PATH"
                    fi

                    echo "Using .NET SDK: $(dotnet --version)"
                    dotnet --info
                    dotnet restore HisEmrService/HisEmrService.csproj --disable-parallel
                    dotnet build HisEmrService/HisEmrService.csproj --configuration Release --no-restore -m:1
                '''
            }
        }

        stage('3. Static Code Analysis') {
            when {
                expression { params.RUN_SONAR }
            }
            steps {
                echo '=== STEP 2: SCANNING CODE QUALITY WITH SONARQUBE ==='
                withSonarQubeEnv('SonarQube-Server') {
                    // Gọi trực tiếp đường dẫn bin của bộ quét để xử lý triệt để lỗi exit code 127
                    sh "${SCANNER_HOME}/bin/sonar-scanner -Dsonar.projectKey=HIS-Hospital-Information-System -Dsonar.sources=. -Dsonar.sourceEncoding=UTF-8"
                }
            }
        }
    }

    post {
        success {
            echo '=== CI PIPELINE EXECUTED SUCCESSFULLY ==='
            script {
                    try {
                        slackSend(
                            tokenCredentialId: env.SLACK_CREDENTIALS_ID,
                            channel: env.SLACK_CHANNEL,
                            color: 'good',
                            failOnError: false,
                            message: "🟢 BÁO CÁO: Luồng build ${env.JOB_NAME} [Số #${env.BUILD_NUMBER}] đã thành công. ${env.BUILD_URL}"
                        )
                    } catch (err) {
                        echo "Slack success notification failed: ${err.message}"
                    }
            }
        }
        failure {
            echo '=== CI PIPELINE FAILED AT SOME STAGES ==='
            script {
                try {
                    slackSend(
                        tokenCredentialId: env.SLACK_CREDENTIALS_ID,
                        channel: env.SLACK_CHANNEL,
                        color: 'danger',
                        failOnError: false,
                        message: "🔴 CẢNH BÁO: Luồng build ${env.JOB_NAME} [Số #${env.BUILD_NUMBER}] thất bại. Vui lòng đối soát Console Output. ${env.BUILD_URL}"
                    )
                } catch (err) {
                    echo "Slack failure notification failed: ${err.message}"
                }
            }
        }
    }
}
