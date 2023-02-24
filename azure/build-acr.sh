while getopts i:r:s: flag
do
    case "${flag}" in
        i) image=${OPTARG};;
        r) registry=${OPTARG};;
        s) source=${OPTARG};;
    esac
done

az acr build \
    --registry $registry \
    --image $image \
    $source