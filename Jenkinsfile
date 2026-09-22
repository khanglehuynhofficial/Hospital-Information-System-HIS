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
                    if command -v dotnet >/dev/null 2>&1; then
                        echo 'Using the .NET SDK installed on the Jenkins agent.'
                        dotnet --version
                        dotnet restore HisEmrService/HisEmrService.csproj
                        dotnet build HisEmrService/HisEmrService.csproj --configuration Release --no-restore
                    elif command -v docker >/dev/null 2>&1 && docker info >/dev/null 2>&1; then
                        echo 'Using the .NET 8 SDK Docker image.'
                        docker run --rm \
                            -v "$PWD:/src" \
                            -w /src \
                            mcr.microsoft.com/dotnet/sdk:8.0 \
                            dotnet restore HisEmrService/HisEmrService.csproj
                        docker run --rm \
                            -v "$PWD:/src" \
                            -w /src \
                            mcr.microsoft.com/dotnet/sdk:8.0 \
                            dotnet build HisEmrService/HisEmrService.csproj --configuration Release --no-restore
                    else
                        echo 'ERROR: Jenkins agent has neither dotnet nor a usable Docker daemon.'
                        exit 127
                    fi
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
                slackSend(
                    tokenCredentialId: env.SLACK_CREDENTIALS_ID,
                    channel: env.SLACK_CHANNEL,
                    color: 'good',
                    failOnError: false,
                    message: "🟢 BÁO CÁO: Luồng build ${env.JOB_NAME} [Số #${env.BUILD_NUMBER}] đã thành công. ${env.BUILD_URL}"
                )
            }
        }
        failure {
            echo '=== CI PIPELINE FAILED AT SOME STAGES ==='
            script {
                slackSend(
                    tokenCredentialId: env.SLACK_CREDENTIALS_ID,
                    channel: env.SLACK_CHANNEL,
                    color: 'danger',
                    failOnError: false,
                    message: "🔴 CẢNH BÁO: Luồng build ${env.JOB_NAME} [Số #${env.BUILD_NUMBER}] thất bại. Vui lòng đối soát Console Output. ${env.BUILD_URL}"
                )
            }
        }
    }
}
