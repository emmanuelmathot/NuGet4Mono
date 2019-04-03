pipeline {
  parameters{
    booleanParam(name: 'NUGET_PUBLISH', defaultValue: false, description: 'If this parameter is set, the build will try to publish the artifact to NuGet.', )
    string(name: 'NUGET_API_KEY', defaultValue: '', description: 'Head over to http://nuget.org/ and register for an account. Once you do that, click on "My Account" to see an API Key that was generated for you.', )
    choice(name: 'DOTNET_CONFIG', choices: "Debug\nRelease", description: 'Debug will produce symbols in the assmbly to be able to debug it at runtime. This is the recommended option for feature, hotfix testing or release candidate.<br/><strong>For publishing a release from master branch, please choose Release.</strong>', )
   }
  agent { node { label 'centos7-mono' } }
  stages {
    stage('Init') {
      steps {
        sh 'sudo yum -y install rpm-build redhat-rpm-config rpmdevtools yum-utils'
        sh 'mkdir -p $WORKSPACE/build/{BUILD,RPMS,SOURCES,SPECS,SRPMS}'
        sh 'cp nuget4mono.spec $WORKSPACE/build/SPECS/nuget4mono.spec'
        sh 'spectool -g -R --directory $WORKSPACE/build/SOURCES $WORKSPACE/build/SPECS/nuget4mono.spec'
        script {
          def sdf = sh(returnStdout: true, script: 'date -u +%Y%m%dT%H%M%S').trim()
          if (env.BRANCH_NAME == 'master') 
            env.release = env.BUILD_NUMBER
          else
            env.release = "SNAPSHOT" + sdf
        }
      }
    }
    stage('Build') {
      steps {
        echo "Build .NET application"
        sh 'nuget restore -MSBuildVersion 14'
        sh "xbuild /p:Configuration=Release"
        sh 'cp -r NuGet4Mono/bin $WORKSPACE/build/SOURCES/'
        sh 'cp src/main/scripts/nuget4mono $WORKSPACE/build/SOURCES/'
        sh 'cp -r packages $WORKSPACE/build/SOURCES/'
      }
    }
    stage('Package') {
      steps {
        echo "Build nuget packages"
        sh "mono NuGet4Mono/bin/NuGet4Mono.exe -g origin/${env.BRANCH_NAME} -p ${workspace}/NuGet4Mono/packages.config ${workspace}/NuGet4Mono/bin/NuGet4Mono.exe"
        sh 'cat *.nuspec'
        sh 'nuget pack NuGet4Mono.nuspec -OutputDirectory build'
        sh 'nuget pack NuGet4Mono.Extensions.nuspec -OutputDirectory build'
        sh "echo ${params.NUGET_PUBLISH}"
        echo "Build package dependencies"
        sh "sudo yum-builddep -y $WORKSPACE/build/SPECS/nuget4mono.spec"
        echo "Build package"
        sh "sudo rpmbuild --define \"_topdir $WORKSPACE/build\" -ba --define '_branch ${env.BRANCH_NAME}' --define '_release ${env.release}' $WORKSPACE/build/SPECS/nuget4mono.spec"
        sh "rpm -qpl $WORKSPACE/build/RPMS/*/*.rpm"
      }
    }
    stage('Nuget-Publish') {
      when {
        expression {
          return params.NUGET_PUBLISH
        }
      }
      steps {
        echo 'Deploying'
        sh "nuget push build/NuGet4Mono.*.nupkg -ApiKey ${params.NUGET_API_KEY} -Source https://www.nuget.org/api/v2/package"
        sh "nuget push build/NuGet4Mono.Extensions.*.nupkg -ApiKey ${params.NUGET_API_KEY} -Source https://www.nuget.org/api/v2/package"
      }       
    }
    stage('Publish') {
      steps {
        echo 'Deploying'
        script {
            // Obtain an Artifactory server instance, defined in Jenkins --> Manage:
            def server = Artifactory.server "repository.terradue.com"

            // Read the upload specs:
            def uploadSpec = readFile 'artifactdeploy.json'

            // Upload files to Artifactory:
            def buildInfo = server.upload spec: uploadSpec

            // Publish the merged build-info to Artifactory
            server.publishBuildInfo buildInfo
        }
      }       
    }
  }
}